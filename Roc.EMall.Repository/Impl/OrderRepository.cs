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
            var existingEntry = await Database.QueryFirstOrDefaultAsync("SELECT `con_version` FROM `order` WHERE `order_id`=@OrderId", order);
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
                values(@OrderId,@Owner,@RecipientName,@RecipientPhone,@RecipientAddress,@Amount,@Status,@CreatedAt)";
        const string insertItemSql = @"insert into `order_line_item` (`order_id`,`goods_id`,`goods_name`,`quantity`,`amount`) value(@OrderId,@GoodsId,@GoodsName,@Quantity,@Amount)";
        private async ValueTask InsertAsync(Order order)
        {
            await Database.ExecuteAsync(insertOrderSql, new
            {
                order.OrderId, order.Owner,
                RecipientName = order.Recipient.Name, RecipientPhone = order.Recipient.PhoneNumber, RecipientAddress = order.Recipient.Address,
                order.Amount,Status=order.Status.ToString(), CreatedAt = DateTime.Now
            },transaction:Transaction);
            
            var data = order.Items.Select(item => new { order.OrderId, item.GoodsId,item.GoodsName, item.Quantity, item.Amount }).ToArray();
            await Database.ExecuteAsync(insertItemSql, data,Transaction);
        }

        const string updateOrderSql = @"update `order` set `con_version`=`con_version` + 1,
                   `status`=@Status,
                   `transaction_id`=@TransactionId,paid_time=@PaidTime,
                   `package_id`=@PackageId,`express_no`=@ExpressNo,
                   `canceled_time`=@CanceledTime
                where `order_id`=@OrderId AND `con_version`=@ConVersion";
        private async ValueTask UpdateAsync(Order order, int conVersion)
        {
            await Database.ExecuteAsync(updateOrderSql, new { order.OrderId,ConVersion = conVersion, 
                Status=order.Status.ToString(),
                order.Payment.TransactionId,order.Payment.PaidTime,
                order.Express.PackageId,order.Express.ExpressNo,
                order.CanceledTime,
            },Transaction);
        }
    }
}