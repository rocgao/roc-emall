namespace Roc.EMall.Domain.Event
{
    public class OrderPackedEvent:IDomainEvent
    {
        public OrderPackedEvent(long eventId,long packageId, long orderId)
        {
            EventId = eventId;
            PackageId = packageId;
            OrderId = orderId;
        }

        public long EventId { get; }
        public long PackageId { get; }
        public long OrderId { get; }
    }
}