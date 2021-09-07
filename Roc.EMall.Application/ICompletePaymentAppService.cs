using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface ICompletePaymentAppService
    {
        ValueTask CompleteAsync(long transactionId);
    }
}