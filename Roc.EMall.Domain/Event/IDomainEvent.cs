using MediatR;

namespace Roc.EMall.Domain.Event
{
    public interface IDomainEvent:INotification
    {
        long EventId { get; }
    }
}