using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;
using Roc.EMall.Domain.OrderContext;

namespace Roc.EMall.Repository.Impl
{
    class OrderQueryRepository:QueryRepositoryBase,IOrderQueryRepository
    {
        public OrderQueryRepository(IConfiguration config, IDbConnectionFactory dbConnectionFactory) : base(config, dbConnectionFactory)
        {
        }

        public async ValueTask<Order> GetAsync(long orderId)
        {
            var entity = await Database.QueryFirstOrDefaultAsync("select * from `order` where `order_id`=@orderId", new { orderId });
            if (entity == null)
            {
                return null;
            }

            var entityItems = await Database.QueryAsync("select * from `order_line_item` where `order_id`=@orderId", new { orderId });
            var orderLineItems= entityItems.Select(it => new LineItem(it.goods_id, it.quantity, it.amount)).ToArray();

            return new Order(entity.order_id, new OwnerInfo(entity.owner_id),
                new RecipientInfo(entity.recipient_name, entity.recipient_phone, entity.recipient_address), entity.amount,orderLineItems);
        }
    }
}