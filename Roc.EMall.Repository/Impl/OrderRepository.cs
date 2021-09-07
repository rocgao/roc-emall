using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.OrderContext;

namespace Roc.EMall.Repository.Impl
{
    class OrderRepository:RepositoryBase,IOrderRepository
    {
        public async ValueTask<Order> GetAsync(long orderId)
        {
            var entity = await Database.QueryFirstOrDefaultAsync("select * from `order` where `order_id`=@orderId", new { orderId },Transaction);
            if (entity == null)
            {
                return null;
            }

            return await LoadOrderAsync(entity);
        }
        
        private async ValueTask<Order> LoadOrderAsync(dynamic entity)
        {
            var entityItems = await Database.QueryAsync("select * from `order_line_item` where `order_id`=@orderId", new { orderId=entity.order_id },Transaction);
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
        
        public async ValueTask StoreAsync(Order order)
        {
            var existingEntry = await Database.QueryFirstOrDefaultAsync("SELECT `con_version` FROM `order` WHERE `order_id`=@Id", order);
            if (existingEntry == null)
            {
                await InsertAsync(order);
            }
            else
            {
                await UpdateAsync(order, existingEntry.con_version);
            }
        }

        const string insertOrderSql = @"insert into `order` (`order_id`,`owner_id`,`recipient_name`,`recipient_phone`,`recipient_address`,`amount`,`status`,`created_at`) 
                values(@Id,@Owner,@RecipientName,@RecipientPhone,@RecipientAddress,@Amount,@Status,@CreatedAt)";
        const string insertItemSql = @"insert into `order_line_item` (`order_id`,`goods_id`,`goods_name`,`quantity`,`amount`) value(@Id,@GoodsId,@GoodsName,@Quantity,@Amount)";
        private async ValueTask InsertAsync(Order order)
        {
            await Database.ExecuteAsync(insertOrderSql, new
            {
                order.Id, order.Owner,
                RecipientName = order.Recipient.Name, RecipientPhone = order.Recipient.PhoneNumber, RecipientAddress = order.Recipient.Address,
                order.Amount,Status=order.Status.ToString(), CreatedAt = DateTime.Now
            },transaction:Transaction);
            
            var data = order.Items.Select(item => new {order.Id, item.GoodsId,item.GoodsName, item.Quantity, item.Amount }).ToArray();
            await Database.ExecuteAsync(insertItemSql, data,Transaction);
        }

        const string updateOrderSql = @"update `order` set `con_version`=`con_version` + 1,
                   `status`=@Status,
                   `transaction_id`=@TransactionId,paid_time=@PaidTime,
                   `package_id`=@PackageId,`express_no`=@ExpressNo,
                   `canceled_time`=@CanceledTime
                where `order_id`=@Id AND `con_version`=@ConVersion";
        private async ValueTask UpdateAsync(Order order, int conVersion)
        {
            await Database.ExecuteAsync(updateOrderSql, new {
                order.Id,ConVersion = conVersion, 
                Status=order.Status.ToString(),
                order.Payment.TransactionId,order.Payment.PaidTime,
                order.Express.PackageId,order.Express.ExpressNo,
                order.CanceledTime,
            },Transaction);
        }
    }
}