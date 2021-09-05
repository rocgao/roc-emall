using System.Threading.Tasks;
using MediatR;
using Roc.EMall.Domain.Event;

namespace Roc.EMall.Application.Components
{
    class DomainEventPublisher:IDomainEventPublisher
    {
        private readonly IMediator _mediator;

        public DomainEventPublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ValueTask PublishAsync<TEvent>(params TEvent[] events) where TEvent : class, IDomainEvent
        {
            foreach (var domainEvent in events)
            {
                _mediator.Publish(domainEvent);
            }
            return ValueTask.CompletedTask;
        }
    }
}