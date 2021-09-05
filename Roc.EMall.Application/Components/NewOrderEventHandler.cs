using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Roc.EMall.Domain.Event;

namespace Roc.EMall.Application.Components
{
    class NewOrderEventHandler:INotificationHandler<NewOrderEvent>
    {
        private readonly ILogger<NewOrderEventHandler> _logger;

        public NewOrderEventHandler(ILogger<NewOrderEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(NewOrderEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Event:{notification.EventId} -> new order:{notification.OrderId}");
            return Task.CompletedTask;
        }
    }
}