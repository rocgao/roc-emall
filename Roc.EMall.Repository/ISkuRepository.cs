using System.Threading.Tasks;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository
{
    public interface ISkuRepository:IRepository
    {
        ValueTask StoreAsync(Sku[] skuArray);
        ValueTask<bool> OccupyAsync(long skuId,long orderId, int quantity,string @operator);

        ValueTask<bool> UseAsync(long orderId, string @operator);

        ValueTask<bool> UndoOccupyingAsync(long orderId, string @operator);
    }
}