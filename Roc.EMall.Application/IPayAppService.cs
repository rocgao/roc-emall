using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface IPayAppService
    {
        ValueTask PayAsync(long transactionId);
    }
}