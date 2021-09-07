using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;

namespace Roc.EMall.Application.Components
{
    class PackageDeliveredEventHandler:HandlerBase<PackageDeliveredEvent>
    {
        private readonly IUOWFactory _uowFactory;

        public PackageDeliveredEventHandler(ILogger<PackageDeliveredEventHandler> logger, IUOWFactory uowFactory) : base(logger)
        {
            _uowFactory = uowFactory;
        }

        protected override async Task InternalHandle(PackageDeliveredEvent notification, CancellationToken cancellationToken)
        {
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            
            var order = await repo.GetAsync(notification.OrderId) ?? throw new ArgumentNullException($"订单不存在！{notification.OrderId}");
            order.Deliver(notification.ExpressNo);
            
            await repo.StoreAsync(order);
            uow.Commit();
        }
    }
}