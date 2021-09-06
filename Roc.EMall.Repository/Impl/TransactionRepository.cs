using System;
using System.Threading.Tasks;
using Dapper;
using Roc.EMall.Domain.PaymentContext;

namespace Roc.EMall.Repository.Impl
{
    class TransactionRepository : RepositoryBase, ITransactionRepository
    {
        public async ValueTask<PaymentTransaction> GetAsync(long transactionId)
        {
            var entry = await Database.QueryFirstOrDefaultAsync("SELECT * FROM `payment_transaction` WHERE `id`=@transactionId", new{transactionId}, transaction: Transaction);
            if (null == entry)
            {
                return null;
            }
            
            return new PaymentTransaction(entry.id)
            {
                BusinessId = entry.business_id,
                Amount = entry.amount,
                CreatedAt = entry.created_at,
                IsPaid = entry.is_paid != 0,
                PaidTime = entry.paid_time,
                IsCanceled = entry.is_canceled != 0,
                CanceledTime = entry.cancel_time,
            };
        }

        public async ValueTask StoreAsync(PaymentTransaction model)
        {
            var insertTransactionSql = @"insert into `payment_transaction` (`id`,`business_id`,`amount`,`created_at`) 
                 values(@Id,@BusinessId,@Amount,@CreatedAt)";
            var updateTransactionSql = @"update `payment_transaction` set `con_version`=`con_version`+1,`is_paid`=@IsPaid,`paid_time`=@PaidTime,
                                 `is_canceled`=@IsCanceled,`canceled_time`=@CanceledTime 
                 where `id`=@Id AND `con_version`=@ConVersion";

            var existing = await Database.QueryFirstOrDefaultAsync("SELECT * FROM `payment_transaction` WHERE `id`=@Id", model, transaction: Transaction);
            if (existing == null)
            {
                await Database.ExecuteAsync(insertTransactionSql, new { model.Id, model.BusinessId, model.Amount, CreatedAt = DateTime.Now });
            }
            else
            {
                var affectedRows = await Database.ExecuteAsync(updateTransactionSql, new { model.Id, model.IsPaid, model.PaidTime,model.IsCanceled,model.CanceledTime, ConVersion = existing.con_version });
                if (affectedRows != 1)
                {
                    throw new InvalidOperationException("更新行失败");
                }
            }
        }
    }
}