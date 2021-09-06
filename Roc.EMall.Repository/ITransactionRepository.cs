using System.Threading.Tasks;
using Roc.EMall.Domain.PaymentContext;

namespace Roc.EMall.Repository
{
    public interface ITransactionRepository:IRepository
    {
        ValueTask StoreAsync(PaymentTransaction model);
    }
}