using System.Threading.Tasks;
using Roc.EMall.Domain.PriceContext;

namespace Roc.EMall.Repository
{
    public interface IGoodsPriceQueryRepository:IQueryRepository
    {
        ValueTask<GoodsPrice[]> GetGoodsByIdAsync(params long[] goodsId);
    }
}