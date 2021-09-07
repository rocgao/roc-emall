using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;

namespace Roc.EMall.Application.Components
{
    class OrderPackedEventHandler:HandlerBase<OrderPackedEvent>
    {
        private readonly IUOWFactory _uowFactory;

        public OrderPackedEventHandler(ILogger<OrderPackedEventHandler> logger,IUOWFactory uowFactory) : base(logger)
        {
            _uowFactory = uowFactory;
        }

        protected override async Task InternalHandle(OrderPackedEvent notification, CancellationToken cancellationToken)
        {
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            
            var order = await repo.GetAsync(notification.OrderId)??throw new ArgumentNullException($"订单不存在！orderId:{notification.OrderId}");
            order.Pack(notification.PackageId);
            
            await repo.StoreAsync(order);
            uow.Commit();
        }
    }
}