using System.Threading.Tasks;
using Dapper;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;
using Roc.EMall.Domain.PaymentContext;

namespace Roc.EMall.Repository.Impl
{
    class TransactionQueryRepository : QueryRepositoryBase, ITransactionQueryRepository
    {
        public TransactionQueryRepository(IConfiguration config, IDbConnectionFactory dbConnectionFactory) : base(config, dbConnectionFactory)
        {
        }

        public async ValueTask<PaymentTransaction> GetByBusinessIdAsync(string businessId)
        {
            var entry = await Database.QueryFirstOrDefaultAsync("select * from `payment_transaction` where `business_id`=@businessId", new { businessId });
            if (entry == null)
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
    }
}