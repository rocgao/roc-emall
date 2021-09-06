using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;

namespace Roc.EMall.Application.Components
{
    class OrderPaidEventHandler:HandlerBase<OrderPaidEvent>
    {
        private readonly IUOWFactory _uowFactory;

        public OrderPaidEventHandler(ILogger<OrderPaidEventHandler> logger,IUOWFactory uowFactory) : base(logger)
        {
            _uowFactory = uowFactory;
        }

        protected override async Task InternalHandle(OrderPaidEvent notification, CancellationToken cancellationToken)
        {
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IPackageRepository>();
            await repo.AddPendingAsync(notification.OrderId);
            uow.Commit();
        }
    }
}