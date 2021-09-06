using MediatR;

namespace Roc.EMall.Domain.Event
{
    public record OrderPaidEvent(long EventId, long OrderId) : IDomainEvent;
}