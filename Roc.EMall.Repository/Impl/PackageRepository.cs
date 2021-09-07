using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository.Impl
{
    class PackageRepository : RepositoryBase, IPackageRepository
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
                new { OrderId = orderId, CreatedAt = DateTime.Now }, Transaction);
        }

        public async ValueTask StoreAsync(Package package)
        {
            var existingEntity = await Database.QueryFirstOrDefaultAsync("SELECT `con_version` FROM `package` WHERE `id`=@Id", package, Transaction);
            if (existingEntity == null)
            {
                await InsertAsync(package);
            }
            else
            {
                await UpdateAsync(package, existingEntity.con_version);
            }
        }

        private const string insertPackageSql = @"insert into `package` (`id`,`order_id`,`recipient_name`,`recipient_phone`,`recipient_address`,`created_at`) 
            values(@Id,@OrderId,@RecipientName,@RecipientPhone,@RecipientAddress,@CreatedAt)";

        private async ValueTask InsertAsync(Package package)
        {
            await Database.ExecuteAsync(insertPackageSql, new
            {
                package.Id, package.OrderId, package.RecipientName, package.RecipientPhone, package.RecipientAddress, CreatedAt = DateTime.Now,
            }, Transaction);
            await Database.ExecuteAsync("INSERT INTO `package_line_item` VALUES(@PackageId,@Id,@GoodsId,@GoodsName,@GoodsQuantity)", package.Items, Transaction);
        }

        private const string updatePackageSql = @"update `package` set `con_version`=`con_version` + 1,`is_delivered`=@IsDelivered,`delivering_time`=@DeliveringTime,
            `express_no`=@ExpressNo,`is_signed`=@IsSigned,`signing_time`=@SigningTime WHERE `id`=@Id AND `con_version`=@conVersion";

        private async ValueTask UpdateAsync(Package package, int conVersion)
        {
            await Database.ExecuteAsync(updatePackageSql, new
            {
                package.Id, package.IsDelivered, package.DeliveringTime, package.ExpressNo,
                package.IsSigned, package.SigningTime,
                conVersion,
            });
        }
    }
}