using System;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository.Impl
{
    class SkuRepository:RepositoryBase,ISkuRepository
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

        public async ValueTask<bool> OccupyAsync(long skuId, int quantity, string businessId, string @operator)
        {
            var updateSkuSql = "update `sku` set `available`=`available`-@quantity,`occupied`= `occupied` + @quantity where `id`=@skuId AND `available`>=@quantity"; 
            var affectedRow= await Database.ExecuteAsync(updateSkuSql, new {skuId,quantity});
            if (affectedRow != 1)
            {
                return false;
            }

            var insertOpsSql = "insert into `sku_ops_record` (`sku_id`,`quantity`,`business_id`,`operator`,`created_at`) value(@skuId,@quantity,@businessId,@operator,@createdAt)";
            await Database.ExecuteAsync(insertOpsSql, new { skuId, quantity, businessId, @operator,createdAt=DateTime.Now });
            
            return true;
        }
    }
}