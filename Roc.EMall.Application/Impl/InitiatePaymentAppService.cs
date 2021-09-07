using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.PaymentContext;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class InitiatePaymentAppService:IInitiatePaymentAppService
    {
        private readonly IOrderQueryRepository _orderQueryRepository;
        private readonly IdWorker _idWorker;
        private readonly ITransactionQueryRepository _transactionQueryRepository;
        private readonly IUOWFactory _uowFactory;

        public InitiatePaymentAppService(IOrderQueryRepository orderQueryRepository,IdWorker idWorker,ITransactionQueryRepository transactionQueryRepository,IUOWFactory uowFactory)
        {
            _orderQueryRepository = orderQueryRepository;
            _idWorker = idWorker;
            _transactionQueryRepository = transactionQueryRepository;
            _uowFactory = uowFactory;
        }

        public async ValueTask<long> InitiateAsync(long orderId)
        {
            var order = await _orderQueryRepository.GetAsync(orderId)??throw new ArgumentNullException($"订单不存在！orderId:{orderId}");
            var existingTransaction= await _transactionQueryRepository.GetByOrderIdAsync(order.OrderId);
            if (existingTransaction != null)
            {
                return existingTransaction.Id;
            }

            var paymentTransaction = new PaymentTransaction(_idWorker.NextId())
            {
                OrderId = order.OrderId,
                Amount = order.Amount
            };
            
            order.InitiatePayment(paymentTransaction.Id);

            // 数据持久化
            using var uow = _uowFactory.Create();
            
            var transactionRepository = uow.CreateRepository<ITransactionRepository>();
            await transactionRepository.StoreAsync(paymentTransaction);

            var orderRepository = uow.CreateRepository<IOrderRepository>();
            await orderRepository.StoreAsync(order);
            
            // 提交事务
            uow.Commit();
            
            return paymentTransaction.Id;
        }
    }
}