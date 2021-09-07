using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
using Roc.EMall.Domain.OrderContext;
using Roc.EMall.Repository;

namespace Roc.EMall.Application.Components
{
    class OrderPackedEventHandler:HandlerBase<OrderPackedEvent>
    {
        private readonly IUOWFactory _uowFactory;
        private readonly IOrderQueryRepository _queryRepository;

        public OrderPackedEventHandler(ILogger<OrderPackedEventHandler> logger,IUOWFactory uowFactory,IOrderQueryRepository queryRepository) : base(logger)
        {
            _uowFactory = uowFactory;
            _queryRepository = queryRepository;
        }

        protected override async Task InternalHandle(OrderPackedEvent notification, CancellationToken cancellationToken)
        {
            var order = await _queryRepository.GetAsync(notification.OrderId)??throw new ArgumentNullException($"订单不存在！orderId:{notification.OrderId}");
            order.Pack(notification.PackageId);

            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            await repo.StoreAsync(order);
            uow.Commit();
        }
    }
}