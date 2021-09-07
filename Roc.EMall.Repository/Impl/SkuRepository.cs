using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.SkuContext;
using Roc.EMall.Infra;

namespace Roc.EMall.Repository.Impl
{
    class SkuRepository : RepositoryBase, ISkuRepository
    {
        
        public async ValueTask<Sku> GetAsync(long skuId,ISkuRepository.LoadOpsRecordOption loadOpsRecordOpt=null)
        {
            var entity = await Database.QueryFirstOrDefaultAsync("SELECT * FROM `sku` WHERE `id`=@skuId",new{skuId},Transaction);
            if (entity == null)
            {
                return null;
            }

            IEnumerable<SkuOpsRecord> opsRecords=null;
            if (loadOpsRecordOpt!=null)
            {
                var opsRecordEntities = await Database.QueryAsync("SELECT * FROM `sku_ops_record` WHERE `sku_id`=@skuId AND `order_id`=@orderId",
                    new { skuId, orderId = loadOpsRecordOpt.OrderId });
                opsRecords = opsRecordEntities.Select(it => new SkuOpsRecord(it.sku_id, it.order_id,
                    it.quantity, it.@operator, Enum.Parse<SkuOpsKind>(it.operation_kind)));
            }
            return new Sku(entity.id, entity.name, entity.available, entity.balance, entity.occupied, entity.used,opsRecords){ConVersion = entity.con_version};
        }

        private const string UpdateSkuSql = @"update `sku` set `con_version`=`con_version`+1,
                 `available`=@Available,`balance`=@Balance,`occupied`=@Occupied,`used`=@Used 
                 where `Id`=@Id AND `con_version`=@ConVersion";

        private const string InsertSkuOpsSql = @"insert into `sku_ops_record` 
        (`sku_id`,`quantity`,`order_id`,`operator`,`operation_kind`,`created_at`) 
        value(@SkuId,@Quantity,@OrderId,@Operator,@OpsKind,@CreatedAt)";
        public async ValueTask StoreAsync(Sku sku)
        {
            var affectedRows = await Database.ExecuteAsync(UpdateSkuSql, sku,Transaction);
            if (affectedRows != 1)
            {
                throw new ConcurrencyUpdateException();
            }

            if (sku.OpsRecords.Count == 0)
            {
                return;
            }

            var opsRecords = sku.OpsRecords.Added.Select(it =>
                new { it.SkuId, it.Operator, it.Quantity, OpsKind = it.OpsKind.ToString(),it.OrderId,CreatedAt=DateTime.Now });
            await Database.ExecuteAsync(InsertSkuOpsSql, opsRecords,Transaction);
        }

        public async ValueTask<bool> OccupyAsync(long skuId, long orderId, int quantity, string @operator)
        {
            var updateSkuSql = "update `sku` set `available`=`available`-@quantity,`occupied`= `occupied` + @quantity where `id`=@skuId AND `available`>=@quantity";
            var affectedRow = await Database.ExecuteAsync(updateSkuSql, new { skuId, quantity });
            if (affectedRow != 1)
            {
                return false;
            }

            var insertOpsSql = "insert into `sku_ops_record` (`sku_id`,`quantity`,`order_id`,`operator`,`operation_kind`,`created_at`) value(@skuId,@quantity,@orderId,@operator,@kind,@createdAt)";
            await Database.ExecuteAsync(insertOpsSql, new { skuId, quantity, orderId, @operator,kind=SkuOpsKind.Occupied.ToString(), createdAt = DateTime.Now });

            return true;
        }

        public async ValueTask<bool> UseAsync(long orderId, string @operator)
        {
            // 验证是否存在Used记录，防止重复扣减
            var existingCount = await Database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM `sku_ops_record` WHERE `order_id`=@orderId AND `operation_kind`=@kind", new { orderId, kind = SkuOpsKind.Used.ToString() }, Transaction);
            if (existingCount > 0)
            {
                return true;
            }
            
            // 查询Occupied记录
            var occupiedOpsRecords= await Database.QueryAsync("SELECT * FROM `sku_ops_record` WHERE `order_id`=@orderId AND `operation_kind`=@kind",new{orderId,kind=SkuOpsKind.Occupied.ToString()},Transaction);
            
            // 根据Occupied记录扣减库存
            await Database.ExecuteAsync(@"UPDATE `sku` set `balance`=`balance`-@quantity,`occupied`=`occupied`-@quantity,`used`=`used` + @quantity
                 WHERE `id`=@sku_id",occupiedOpsRecords, Transaction);
            
            // 生成Used记录
            var usedOpsRecords= occupiedOpsRecords.Select(it => new {it.sku_id,it.quantity,it.order_id,@operator,kind=SkuOpsKind.Used.ToString(), created_at = DateTime.Now }).ToArray();
            await Database.ExecuteAsync("insert into `sku_ops_record` (`sku_id`,`quantity`,`order_id`,`operator`,`operation_kind`,`created_at`) value(@sku_id,@quantity,@order_id,@operator,@kind,@created_at)",
                usedOpsRecords);
            
            return true;
        }

        public async ValueTask<bool> UndoOccupyingAsync(long orderId, string @operator)
        {
            // 验证是否存在Canceled\Used记录，防止重复
            var existingCount = await Database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM `sku_ops_record` WHERE `order_id`=@orderId AND `operation_kind` IN @kinds",
                new { orderId, kinds = new[] { SkuOpsKind.Canceled.ToString(), SkuOpsKind.Used.ToString() } }, Transaction);
            if (existingCount > 0)
            {
                return false;
            }
            
            // 获取Occupied记录
            var occupiedRecords = (await Database.QueryAsync("SELECT * FROM `sku_ops_record` WHERE `order_id`=@orderId AND `operation_kind` IN @kinds",
                new { orderId, Kinds = new[] { SkuOpsKind.Occupied.ToString()} }, Transaction)).ToArray();

            // 调整库存记录
            await Database.ExecuteAsync(@"UPDATE `sku` set `available`=`available`+@quantity,`occupied`=`occupied`-@quantity
                 WHERE `id`=@sku_id", occupiedRecords, Transaction);

            // 生成操作记录
            var canceledOpsRecords= occupiedRecords.Select(it => new {it.sku_id,it.quantity,it.order_id,@operator,kind=SkuOpsKind.Canceled.ToString(), created_at = DateTime.Now }).ToArray();
            await Database.ExecuteAsync("insert into `sku_ops_record` (`sku_id`,`quantity`,`order_id`,`operator`,`operation_kind`,`created_at`) value(@sku_id,@quantity,@order_id,@operator,@kind,@created_at)",
                canceledOpsRecords);

            return true;
        }
    }
}