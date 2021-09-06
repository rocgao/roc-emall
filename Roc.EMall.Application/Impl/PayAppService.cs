using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;

namespace Roc.EMall.Application.Impl
{
    class PayAppService:IPayAppService
    {
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;

        public PayAppService(IUOWFactory uowFactory,IDomainEventPublisher eventPublisher)
        {
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
        }

        public async ValueTask PayAsync(long transactionId)
        {
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<ITransactionRepository>();
            var transaction = await repo.GetAsync(transactionId)??throw new ArgumentNullException($"交易不存在！transactionId:{transactionId}");
            transaction.Pay();
            await repo.StoreAsync(transaction);
            uow.Commit();

            // 发布领域事件
            await _eventPublisher.PublishAsync(transaction.CreatePaymentFinishEvent());
        }
    }
}