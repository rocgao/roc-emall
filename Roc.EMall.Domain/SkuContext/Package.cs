using System;
using Roc.EMall.Domain.Event;

namespace Roc.EMall.Domain.SkuContext
{
    public class Package
    {
        public long Id { get; }
        public long OrderId { get; }
        public string RecipientName { get; }
        public string RecipientPhone { get; }
        public string RecipientAddress { get; }
        public PackageLineItem[] Items { get; }

        public bool IsDelivered { get; private set; }
        public DateTime? DeliveringTime { get; private set; }
        public string ExpressNo { get; private set; }
        public bool IsSigned { get; private set; }
        public DateTime? SigningTime { get; private set; }

        public Package(long id, long orderId, string recipientName, string recipientPhone, string recipientAddress, PackageLineItem[] items,
            bool isDelivered = false, DateTime? deliveringTime = null, string expressNo = null,
            bool isSigned = false, DateTime? signingTime = null)
        {
            Id = id;
            OrderId = orderId;
            RecipientName = recipientName;
            RecipientPhone = recipientPhone;
            RecipientAddress = recipientAddress;
            Items = items;
            IsDelivered = isDelivered;
            DeliveringTime = deliveringTime;
            ExpressNo = expressNo;
            IsSigned = isSigned;
            SigningTime = signingTime;
        }

        public OrderPackedEvent GetOrderPackedEvent(long eventId) => new(eventId, Id,OrderId);

        public void Deliver(string expressNo)
        {
            if (IsDelivered)
            {
                return;
            }

            IsDelivered = true;
            DeliveringTime = DateTime.Now;
            ExpressNo = expressNo;
        }

        public PackageDeliveredEvent GetDeliveredEvent(long eventId) => new(eventId, OrderId, ExpressNo);

        public void Sign()
        {
            if (IsSigned)
            {
                return;
            }

            if (!IsDelivered)
            {
                throw new InvalidOperationException("当前订单还未投递");
            }

            IsSigned = true;
            SigningTime = DateTime.Now;
        }

        public PackageSignedEvent GetSignedEvent(long eventId) => new(eventId, OrderId, SigningTime.Value);
    }

    public record PackageLineItem(long PackageId, int Id, long GoodsId, string GoodsName, int GoodsQuantity);
}