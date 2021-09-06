using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
using Roc.EMall.Domain.OrderContext;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Components
{
    class PaymentFinishEventHandler : HandlerBase<PaymentFinishEvent>
    {
        private readonly ILogger<PaymentFinishEventHandler> _logger;
        private readonly IUOWFactory _uowFactory;
        private readonly IOrderQueryRepository _queryRepository;
        private readonly IdWorker _idWorker;
        private readonly IDomainEventPublisher _eventPublisher;

        public PaymentFinishEventHandler(ILogger<PaymentFinishEventHandler> logger, IUOWFactory uowFactory, 
            IOrderQueryRepository queryRepository,IdWorker idWorker,
            IDomainEventPublisher eventPublisher)
            : base(logger)
        {
            _logger = logger;
            _uowFactory = uowFactory;
            _queryRepository = queryRepository;
            _idWorker = idWorker;
            _eventPublisher = eventPublisher;
        }

        protected override async Task InternalHandle(PaymentFinishEvent notification, CancellationToken cancellationToken)
        {
            var order = await _queryRepository.GetByTransactionAsync(notification.TransactionId);
            if (order == null)
            {
                _logger.LogInformation($"不存在订单！TransactionId:{notification.TransactionId}");
                return;
            }

            order.CompletePayment(notification.TransactionId);

            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            await repo.StoreAsync(order);
            uow.Commit();
            
            // 发布领域事件
            var orderPaidEvent = order.GetOrderPaidEvent(_idWorker.NextId());
            await _eventPublisher.PublishAsync(orderPaidEvent);
        }
    }
}