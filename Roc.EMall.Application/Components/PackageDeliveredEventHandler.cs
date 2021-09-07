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
        private readonly IOrderQueryRepository _orderQueryRepository;
        private readonly IUOWFactory _uowFactory;

        public PackageDeliveredEventHandler(ILogger<PackageDeliveredEventHandler> logger,IOrderQueryRepository orderQueryRepository, IUOWFactory uowFactory) : base(logger)
        {
            _orderQueryRepository = orderQueryRepository;
            _uowFactory = uowFactory;
        }

        protected override async Task InternalHandle(PackageDeliveredEvent notification, CancellationToken cancellationToken)
        {
            var order = await _orderQueryRepository.GetAsync(notification.OrderId) ?? throw new ArgumentNullException($"订单不存在！{notification.OrderId}");
            order.Deliver(notification.ExpressNo);

            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            await repo.StoreAsync(order);
            uow.Commit();
        }
    }
}