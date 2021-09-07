using System.Threading.Tasks;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository
{
    public interface ISkuRepository:IRepository
    {
        public record LoadOpsRecordOption(long OrderId);
        ValueTask<Sku> GetAsync(long skuId,LoadOpsRecordOption loadOpsRecordOpt=null);
        ValueTask StoreAsync(Sku sku);
        ValueTask<bool> OccupyAsync(long skuId,long orderId, int quantity,string @operator);

        ValueTask<bool> UseAsync(long orderId, string @operator);

        ValueTask<bool> UndoOccupyingAsync(long orderId, string @operator);
    }
}