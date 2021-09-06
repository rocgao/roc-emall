using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.PaymentContext;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class PayOrderAppService:IPayOrderAppService
    {
        private readonly IOrderQueryRepository _orderQueryRepository;
        private readonly IdWorker _idWorker;
        private readonly ITransactionQueryRepository _transactionQueryRepository;
        private readonly IUOWFactory _uowFactory;

        public PayOrderAppService(IOrderQueryRepository orderQueryRepository,IdWorker idWorker,ITransactionQueryRepository transactionQueryRepository,IUOWFactory uowFactory)
        {
            _orderQueryRepository = orderQueryRepository;
            _idWorker = idWorker;
            _transactionQueryRepository = transactionQueryRepository;
            _uowFactory = uowFactory;
        }

        public async ValueTask<long> PayAsync(long orderId)
        {
            var order = await _orderQueryRepository.GetAsync(orderId)??throw new ArgumentNullException($"订单不存在！orderId:{orderId}");
            var existingTransaction= await _transactionQueryRepository.GetByBusinessIdAsync(order.BusinessId);
            if (existingTransaction != null)
            {
                return existingTransaction.Id;
            }

            var paymentTransaction = new PaymentTransaction(_idWorker.NextId())
            {
                BusinessId = order.BusinessId,
                Amount = order.Amount
            };

            using var uow = _uowFactory.Create();
            var transactionRepository = uow.CreateRepository<ITransactionRepository>();
            await transactionRepository.StoreAsync(paymentTransaction);
            uow.Commit();
            
            return paymentTransaction.Id;
        }
    }
}