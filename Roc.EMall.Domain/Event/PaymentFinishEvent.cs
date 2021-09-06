namespace Roc.EMall.Domain.Event
{
    public record PaymentFinishEvent(long EventId, long TransactionId, string BusinessId) : IDomainEvent;
}