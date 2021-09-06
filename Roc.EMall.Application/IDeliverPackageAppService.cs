using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface IDeliverPackageAppService
    {
        ValueTask DeliverAsync(long packageId, string expressNo);
    }
}