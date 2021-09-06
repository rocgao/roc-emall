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

            return new Package(entity.id, entity.order_id, entity.recipient_name, entity.recipient_phone, entity.recipient_address, null,
                entity.is_delivered != 0, entity.delivering_time, entity.express_no,
                entity.is_signed != 0, entity.signing_time);
        }
    }
}