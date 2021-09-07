using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface IInitiatePaymentAppService
    {
        ValueTask<long> InitiateAsync(long orderId);
    }
}