using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository.Impl
{
    class SkuRepository : RepositoryBase, ISkuRepository
    {
        public ValueTask StoreAsync(Sku[] skuArray)
        {
            throw new System.NotImplementedException();

            // foreach (var sku in skuArray)
            // {
            //     var updateSql = "update `sku` set `available`=@Available,`occupied`=@Occupied where `id`=@SkuId";
            //     var affectedRow= await Database.ExecuteAsync(updateSql, sku);
            // }
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
    }
}