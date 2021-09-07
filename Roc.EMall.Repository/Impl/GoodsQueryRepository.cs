using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;

namespace Roc.EMall.Repository.Impl
{
    class GoodsQueryRepository:QueryRepositoryBase,IGoodsQueryRepository
    {
        public GoodsQueryRepository(IConfiguration config, IDbConnectionFactory dbConnectionFactory) : base(config, dbConnectionFactory)
        {
        }

        public async ValueTask<(long goodsId, string goodsName)[]> QueryAsync(long[] goodsIdArray)
        {
            var entities = await Database.QueryAsync("SELECT * FROM `goods` WHERE `id` in @goodsIdArray", new { goodsIdArray });
            return entities.Select(it => ((long)it.id, (string)it.name)).ToArray();
        }
    }
}