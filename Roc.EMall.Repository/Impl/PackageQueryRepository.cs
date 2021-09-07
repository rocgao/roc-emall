using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository.Impl
{
    class PackageQueryRepository:QueryRepositoryBase, IPackageQueryRepository
    {
        public PackageQueryRepository(IConfiguration config, IDbConnectionFactory dbConnectionFactory) : base(config, dbConnectionFactory)
        {
        }

        public async ValueTask<Package> GetAsync(long packageId)
        {
            var entity = await Database.QueryFirstOrDefaultAsync("SELECT * FROM `package` WHERE `id`=@packageId", new { packageId });
            if (entity == null)
            {
                return null;
            }

            return await LoadAsync(entity);
        }

        public async ValueTask<Package> GetByOrderIdAsync(long orderId)
        {
            var entity = await Database.QueryFirstOrDefaultAsync("SELECT * FROM `package` WHERE `order_id`=@orderId", new { orderId });
            if (entity == null)
            {
                return null;
            }

            return await LoadAsync(entity);
        }

        private async ValueTask<Package> LoadAsync(dynamic entity)
        {
            var lineItemEntities = await Database.QueryAsync("SELECT * FROM `package_line_item` WHERE `package_id`=@packageId", new { packageId = entity.id });
            var lineItems = lineItemEntities.Select(it => new PackageLineItem(it.package_id, it.id, it.goods_id, it.goods_name, it.goods_quantity)).ToArray();
            return new Package(entity.id, entity.order_id, entity.recipient_name, entity.recipient_phone, entity.recipient_address, lineItems,
                entity.is_delivered != 0, entity.delivering_time, entity.express_no,
                entity.is_signed != 0, entity.signing_time);
        }
    }
}