using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
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
            IOrderQueryRepository queryRepository,
            IdWorker idWorker,
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
            var order = await _queryRepository.GetAsync(notification.OrderId);
            if (order == null)
            {
                _logger.LogInformation($"不存在订单！OrderId:{notification.OrderId}");
                return;
            }

            order.CompletePayment(notification.PaidTime);

            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            var skuRepo = uow.CreateRepository<ISkuRepository>();
            await repo.StoreAsync(order);
            await skuRepo.UseAsync(order.OrderId, "handler");
            uow.Commit();
            
            // 发布领域事件
            var orderPaidEvent = order.GetOrderPaidEvent(_idWorker.NextId());
            await _eventPublisher.PublishAsync(orderPaidEvent);
        }
    }
}