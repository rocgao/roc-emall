using System;

namespace Roc.EMall.Domain.Event
{
    public record PaymentFinishEvent(long EventId, long TransactionId, long OrderId,DateTime PaidTime) : IDomainEvent;
}