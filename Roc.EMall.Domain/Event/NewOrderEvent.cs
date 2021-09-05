
namespace Roc.EMall.Domain.Event
{
    public record NewOrderEvent(long EventId, long OrderId) : IDomainEvent;
}