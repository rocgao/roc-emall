using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface ICancelOrderAppService
    {
        ValueTask CancelAsync(long orderId);
    }
}