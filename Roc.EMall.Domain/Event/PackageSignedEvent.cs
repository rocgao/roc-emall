using System;

namespace Roc.EMall.Domain.Event
{
    public class PackageSignedEvent:IDomainEvent
    {
        public PackageSignedEvent(long eventId,long orderId,DateTime signingTime)
        {
            EventId = eventId;
            OrderId = orderId;
            SigningTime = signingTime;
        }

        public long EventId { get; }
        public long OrderId { get; }
        public DateTime SigningTime { get; private set; }
    }
}