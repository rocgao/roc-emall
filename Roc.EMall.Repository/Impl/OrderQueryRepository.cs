using System;
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

            return await LoadOrderAsync(entity);
        }

        public async ValueTask<Order> GetByTransactionAsync(long transactionId)
        {
            var entity = await Database.QueryFirstOrDefaultAsync("select * from `order` where `transaction_id`=@transactionId", new { transactionId });
            if (entity == null)
            {
                return null;
            }

            return await LoadOrderAsync(entity);
        }

        private async ValueTask<Order> LoadOrderAsync(dynamic entity)
        {
            var entityItems = await Database.QueryAsync("select * from `order_line_item` where `order_id`=@orderId", new { orderId=entity.order_id });
            var orderLineItems= entityItems.Select(it => new LineItem(it.goods_id,it.goods_name, it.quantity, it.amount)).ToArray();

            OrderStatus? orderStatus = null;
            if (Enum.TryParse<OrderStatus>(entity.status, true, out OrderStatus status))
            {
                orderStatus = status;
            }
            return new Order(entity.order_id, entity.owner_id,
                new RecipientInfo(entity.recipient_name, entity.recipient_phone, entity.recipient_address), entity.amount, orderLineItems,
                orderStatus,new PaymentInfo(entity.transaction_id,entity.paid_time),new ExpressInfo(entity.package_id,entity.express_no),
                entity.canceled_time);
        }
    }
}