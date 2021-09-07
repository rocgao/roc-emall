using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;

namespace Roc.EMall.Application.Components
{
    class PackageSignedEventHandler:HandlerBase<PackageSignedEvent>
    {
        private readonly IOrderQueryRepository _orderQueryRepository;
        private readonly IUOWFactory _uowFactory;

        public PackageSignedEventHandler(ILogger<PackageSignedEventHandler> logger,IOrderQueryRepository orderQueryRepository,IUOWFactory uowFactory) : base(logger)
        {
            _orderQueryRepository = orderQueryRepository;
            _uowFactory = uowFactory;
        }

        protected override async Task InternalHandle(PackageSignedEvent notification, CancellationToken cancellationToken)
        {
            var order = await _orderQueryRepository.GetAsync(notification.OrderId)??throw new ArgumentNullException($"订单不存在！orderId:{notification.OrderId}");
            order.Sign();
            
            // 持久化数据
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            await repo.StoreAsync(order);
            uow.Commit();
        }
    }
}