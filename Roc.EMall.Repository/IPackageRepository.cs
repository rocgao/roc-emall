using System.Threading.Tasks;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository
{
    public interface IPackageRepository:IRepository
    {
        ValueTask AddPendingAsync(long orderId);

        ValueTask StoreAsync(Package package);
    }
}