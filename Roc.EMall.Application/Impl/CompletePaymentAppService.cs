using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class CompletePaymentAppService:ICompletePaymentAppService
    {
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;
        private readonly IdWorker _idWorker;

        public CompletePaymentAppService(IUOWFactory uowFactory,IDomainEventPublisher eventPublisher,IdWorker idWorker)
        {
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
            _idWorker = idWorker;
        }

        public async ValueTask CompleteAsync(long transactionId)
        {
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<ITransactionRepository>();
            var transaction = await repo.GetAsync(transactionId)??throw new ArgumentNullException($"交易不存在！transactionId:{transactionId}");
            transaction.Pay();
            await repo.StoreAsync(transaction);
            uow.Commit();

            // 发布领域事件
            await _eventPublisher.PublishAsync(transaction.CreatePaymentFinishEvent(_idWorker.NextId()));
        }
    }
}