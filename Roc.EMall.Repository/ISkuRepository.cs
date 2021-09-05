using System.Threading.Tasks;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository
{
    public interface ISkuRepository:IRepository
    {
        ValueTask StoreAsync(Sku[] skuArray);
        ValueTask<bool> OccupyAsync(long skuId, int quantity, string businessId,string @operator);
    }
}