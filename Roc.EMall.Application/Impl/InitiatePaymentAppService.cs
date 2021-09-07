using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.PaymentContext;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class InitiatePaymentAppService:IInitiatePaymentAppService
    {
        private readonly IdWorker _idWorker;
        private readonly IUOWFactory _uowFactory;

        public InitiatePaymentAppService(IdWorker idWorker,IUOWFactory uowFactory)
        {
            _idWorker = idWorker;
            _uowFactory = uowFactory;
        }

        public async ValueTask<long> InitiateAsync(long orderId)
        {
            using var uow = _uowFactory.Create();
            var orderRepository = uow.CreateRepository<IOrderRepository>();

            //  更新订单信息
            var order = await orderRepository.GetAsync(orderId)??throw new ArgumentNullException($"订单不存在！orderId:{orderId}");
            var transactionId = _idWorker.NextId();
            var (ok,currTransactionId)=order.TryInitiatePayment(transactionId);
            if (!ok)
            {
                return currTransactionId;
            }
            await orderRepository.StoreAsync(order);

            // 存储交易记录
            var transactionRepository = uow.CreateRepository<ITransactionRepository>();
            var paymentTransaction = new PaymentTransaction(transactionId)
            {
                OrderId = order.Id,
                Amount = order.Amount
            };
            await transactionRepository.StoreAsync(paymentTransaction);
            
            // 提交事务
            uow.Commit();
            
            return paymentTransaction.Id;
        }
    }
}