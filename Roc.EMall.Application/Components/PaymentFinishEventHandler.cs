using System;
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
        private readonly IUOWFactory _uowFactory;
        private readonly IdWorker _idWorker;
        private readonly IDomainEventPublisher _eventPublisher;

        public PaymentFinishEventHandler(ILogger<PaymentFinishEventHandler> logger, IUOWFactory uowFactory,
            IdWorker idWorker,
            IDomainEventPublisher eventPublisher)
            : base(logger)
        {
            _uowFactory = uowFactory;
            _idWorker = idWorker;
            _eventPublisher = eventPublisher;
        }

        protected override async Task InternalHandle(PaymentFinishEvent notification, CancellationToken cancellationToken)
        {
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            
            // 完成付款
            var order = await repo.GetAsync(notification.OrderId)??throw new ArgumentNullException($"订单不存在！orderId:{notification.OrderId}");
            order.CompletePayment(notification.PaidTime);
            await repo.StoreAsync(order);

            // 扣减库存
            var skuRepo = uow.CreateRepository<ISkuRepository>();
            foreach (var orderItem in order.Items)
            {
                var sku = await skuRepo.GetAsync(orderItem.GoodsId, new ISkuRepository.LoadOpsRecordOption(order.Id));
                sku.Use(order.Id,orderItem.Quantity,"handler");

                await skuRepo.StoreAsync(sku);
            }
            
            // 提交事务
            uow.Commit();
            
            // 发布领域事件
            var orderPaidEvent = order.GetOrderPaidEvent(_idWorker.NextId());
            await _eventPublisher.PublishAsync(orderPaidEvent);
        }
    }
}