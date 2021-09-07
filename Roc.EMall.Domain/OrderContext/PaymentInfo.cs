using System;

namespace Roc.EMall.Domain.OrderContext
{
    public record PaymentInfo(long? TransactionId, DateTime? PaidTime);
}