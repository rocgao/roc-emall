namespace Roc.EMall.Domain.Event
{
    public class OrderCanceledEvent:IDomainEvent
    {
        public OrderCanceledEvent(long eventId,long orderId)
        {
            EventId = eventId;
            OrderId = orderId;
        }

        public long EventId { get; }
        public long OrderId { get; }
    }
}