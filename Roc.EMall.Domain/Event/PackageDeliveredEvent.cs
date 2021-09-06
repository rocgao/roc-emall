namespace Roc.EMall.Domain.Event
{
    public class PackageDeliveredEvent:IDomainEvent
    {
        public PackageDeliveredEvent(long eventId,long orderId, string expressNo)
        {
            EventId = eventId;
            OrderId = orderId;
            ExpressNo = expressNo;
        }

        public long EventId { get; }
        public long OrderId { get; }
        public string ExpressNo { get; }
    }
}