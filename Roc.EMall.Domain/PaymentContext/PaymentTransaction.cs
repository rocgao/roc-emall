using System;

namespace Roc.EMall.Domain.PaymentContext
{
    public class PaymentTransaction
    {
        public PaymentTransaction(long id)
        {
            Id = id;
        }
        public long Id { get; }
        public string BusinessId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidTime { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime? CanceledTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}