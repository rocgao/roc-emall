using System.Threading.Tasks;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository
{
    public interface IPackageQueryRepository:IQueryRepository
    {
        ValueTask<Package> GetAsync(long packageId);
    }
}