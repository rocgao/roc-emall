using System;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository.Impl
{
    class PackageRepository:RepositoryBase,IPackageRepository
    {
        public async ValueTask<PackagePending> GetAsync(long orderId)
        {
            var entity = await Database.QueryFirstOrDefaultAsync("SELECT * FROM `package_pending` WHERE `order_id`=@orderId", new { orderId });
            if (entity == null)
            {
                return null;
            }

            return new PackagePending(entity.order_id, entity.is_packed != 0);
        }
        public async ValueTask AddPendingAsync(long orderId)
        {
            var entity = await GetAsync(orderId);
            if (null != entity)
            {
                return;
            }
            
            await Database.ExecuteAsync("insert into `package_pending` (`order_id`,`created_at`) values(@OrderId,@CreatedAt)",
                new { OrderId = orderId, CreatedAt = DateTime.Now },Transaction);
        }
    }
}