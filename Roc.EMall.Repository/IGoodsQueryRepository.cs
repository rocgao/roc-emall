using System.Threading.Tasks;

namespace Roc.EMall.Repository
{
    public interface IGoodsQueryRepository:IQueryRepository
    {
        ValueTask<(long goodsId, string goodsName)[]> QueryAsync(long[] goodsIdArray);
    }
}