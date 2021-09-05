using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.OrderContext;

namespace Roc.EMall.Repository.Impl
{
    class OrderRepository:RepositoryBase,IOrderRepository
    {
        public async ValueTask StoreAsync(Order order)
        {
            var insertOrderSql = @"insert into `order` (`order_id`,`owner_id`,`recipient_name`,`recipient_phone`,`recipient_address`,`amount`,`status`,`created_at`) 
                values(@OrderId,@OwnerId,@RecipientName,@RecipientPhone,@RecipientAddress,@Amount,@Status,@CreatedAt)";
            await Database.ExecuteAsync(insertOrderSql, new
            {
                order.OrderId, order.Owner.OwnerId,
                RecipientName = order.Recipient.Name, RecipientPhone = order.Recipient.PhoneNumber, RecipientAddress = order.Recipient.Address,
                order.Amount,Status=order.Status.ToString(), CreatedAt = DateTime.Now
            },transaction:Transaction);

            var insertItemSql = @"insert into `order_line_item` (`order_id`,`goods_id`,`quantity`,`amount`) value(@OrderId,@GoodsId,@Quantity,@Amount)";
            var data = order.Items.Select(item => new { order.OrderId, item.GoodsId, item.Quantity, item.Amount }).ToArray();
            await Database.ExecuteAsync(insertItemSql, data,Transaction);
        }
    }
}