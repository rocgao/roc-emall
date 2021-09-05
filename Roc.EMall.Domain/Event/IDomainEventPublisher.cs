using System.Threading.Tasks;

namespace Roc.EMall.Domain.Event
{
    public interface IDomainEventPublisher
    {
        ValueTask PublishAsync<TEvent>(params TEvent[] events) where TEvent:class,IDomainEvent;
    }
}