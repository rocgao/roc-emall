using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;

namespace Roc.EMall.Application.Components
{
    class OrderCanceledEventHandler:HandlerBase<OrderCanceledEvent>
    {
        private readonly IUOWFactory _uowFactory;

        public OrderCanceledEventHandler(ILogger<OrderCanceledEventHandler> logger,IUOWFactory uowFactory) : base(logger)
        {
            _uowFactory = uowFactory;
        }

        protected override async Task InternalHandle(OrderCanceledEvent notification, CancellationToken cancellationToken)
        {
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<ISkuRepository>();
            var result=await repo.UndoOccupyingAsync(notification.OrderId, "Handler");
            uow.Commit();
            Logger.LogInformation($"UndoOccupyingAsync return {result.ToString()}");
        }
    }
}