using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Roc.EMall.Application.Components
{
    abstract class HandlerBase<T>:INotificationHandler<T> where T:INotification
    {
        private readonly ILogger _logger;

        protected ILogger Logger => _logger; 

        protected HandlerBase(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Handle(T notification, CancellationToken cancellationToken)
        {
            try
            {
                await InternalHandle(notification, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(0,e,e.Message);
            }
        }

        protected abstract Task InternalHandle(T notification, CancellationToken cancellationToken);
    }
}