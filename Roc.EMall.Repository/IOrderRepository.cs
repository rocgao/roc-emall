using System.Threading.Tasks;
using Roc.EMall.Domain.OrderContext;

namespace Roc.EMall.Repository
{
    public interface IOrderRepository:IRepository
    {
        ValueTask StoreAsync(Order order);
    }
}