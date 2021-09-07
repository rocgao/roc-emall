using System.Threading.Tasks;
using Roc.EMall.Domain.OrderContext;

namespace Roc.EMall.Repository
{
    public interface IOrderRepository:IRepository
    {
        ValueTask<Order> GetAsync(long orderId);
        ValueTask StoreAsync(Order order);
    }
}