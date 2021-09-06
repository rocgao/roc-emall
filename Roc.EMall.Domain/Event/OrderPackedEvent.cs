namespace Roc.EMall.Domain.Event
{
    public class OrderPackedEvent:IDomainEvent
    {
        public OrderPackedEvent(long eventId,long orderId)
        {
            EventId = eventId;
            OrderId = orderId;
        }

        public long EventId { get; }
        public long OrderId { get; }
    }
}