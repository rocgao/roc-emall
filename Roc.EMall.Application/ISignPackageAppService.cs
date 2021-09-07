using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface ISignPackageAppService
    {
        ValueTask SignAsync(long packageId);
    }
}