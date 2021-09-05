using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;
using Roc.EMall.Domain.PriceContext;

namespace Roc.EMall.Repository.Impl
{
    class GoodsPriceQueryRepository:QueryRepositoryBase, IGoodsPriceQueryRepository
    {
        public GoodsPriceQueryRepository(IConfiguration config,IDbConnectionFactory factory) : base(config,factory)
        {
        }
        
        public async ValueTask<GoodsPrice[]> GetGoodsByIdAsync(params long[] goodsId)
        {
            var entities = (await Database.QueryAsync("SELECT * FROM `goods_price` WHERE `goods_id` IN @goodsId", new{goodsId})).ToArray();
            if (entities.Length != goodsId.Length)
            {
                throw new InvalidOperationException($"未查询到全部Goods。Expected:{goodsId.Length} Actual:{entities.Length}");
            }

            return entities.Select(it => new GoodsPrice(it.goods_id,it.price)).ToArray();
        }
    }
}