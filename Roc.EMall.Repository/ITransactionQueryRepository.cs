using System.Threading.Tasks;
using Roc.EMall.Domain.PaymentContext;

namespace Roc.EMall.Repository
{
    public interface ITransactionQueryRepository:IQueryRepository
    {
        ValueTask<PaymentTransaction> GetByBusinessIdAsync(string businessId);
    }
}