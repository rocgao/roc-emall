using System.Threading.Tasks;
using Roc.EMall.Domain.OrderContext;

namespace Roc.EMall.Repository
{
    public interface IOrderQueryRepository:IQueryRepository
    {
        ValueTask<Order> GetAsync(long orderId);
        ValueTask<Order> GetByTransactionAsync(long transactionId);
    }
}