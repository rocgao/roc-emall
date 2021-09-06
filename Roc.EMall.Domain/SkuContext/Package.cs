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
        public (long goodsId, int quantity)[] Items { get; }

        public bool IsDelivered { get; private set; }
        public DateTime? DeliveringTime { get; private set; }
        public string ExpressNo { get; private set; }
        public bool IsSigned { get; private set; }
        public DateTime? SigningTime { get; private set; }

        public Package(long id,long orderId, string recipientName, string recipientPhone, string recipientAddress, (long goodsId, int quantity)[] items,
            bool isDelivered=false,DateTime? deliveringTime=null,string expressNo=null,
            bool isSigned=false,DateTime? signingTime=null)
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

        public OrderPackedEvent GetOrderPackedEvent(long eventId) => new OrderPackedEvent(eventId, OrderId);

        public void Deliver(string expressNo)
        {
            if (IsDelivered)
            {
                return;
            }

            IsDelivered = true;
            DeliveringTime=DateTime.Now;
            ExpressNo = expressNo;
        }

        public PackageDeliveredEvent GetDeliveredEvent(long eventId) => new PackageDeliveredEvent(eventId, OrderId,ExpressNo);

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
        
        public PackageSignedEvent GetSignedEvent(long eventId) => new PackageSignedEvent(eventId, OrderId,SigningTime.Value);

    }
}