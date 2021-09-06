using System.Threading.Tasks;

namespace Roc.EMall.Repository
{
    public interface IPackageRepository:IRepository
    {
        ValueTask AddPendingAsync(long orderId);
    }
}