using System.Threading.Tasks;
using Roc.EMall.Domain.SkuContext;

namespace Roc.EMall.Repository
{
    public interface ISkuQueryRepository
    {
        ValueTask<Sku[]> GetByIdsAsync(long[] idArray);
    }
}