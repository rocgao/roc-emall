using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface IPayOrderAppService
    {
        ValueTask<long> PayAsync(long orderId);
    }
}