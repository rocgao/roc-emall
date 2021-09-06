using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface IPackAppService
    {
        ValueTask<long> CreateAsync(long orderId);
    }
}