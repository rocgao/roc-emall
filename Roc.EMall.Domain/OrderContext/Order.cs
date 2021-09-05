using System;
using System.Collections.Generic;
using Roc.EMall.Domain.Event;

namespace Roc.EMall.Domain.OrderContext
{
    public partial class Order
    {
        private readonly string _businessId;
        private static readonly Dictionary<OrderStatus, IOrderStatusHandler> statusHandlers=new()
        {
            { OrderStatus.Submitted ,new SubmittedStatusHandler()}
        };
        
        public Order(long orderId, OwnerInfo owner, RecipientInfo recipient, decimal amount,LineItem[] items)
        {
            _businessId = orderId.ToString();
            OrderId = orderId;
            Owner = owner;
            Recipient = recipient;
            Amount = amount;
            Items = items;
        }

        public string BusinessId => _businessId;
        public long OrderId { get; }
        public OwnerInfo Owner { get; }
        public RecipientInfo Recipient { get; }
        public decimal Amount { get; }
        public LineItem[] Items { get; }
        public OrderStatus? Status { get; private set; }

        public NewOrderEvent GetNewOrderEvent() => new NewOrderEvent(OrderId, OrderId);

        public void ChangeStatus(OrderStatus status)
        {
            if (statusHandlers.TryGetValue(status, out var handler))
            {
                handler.Handle(this);
                return;
            }

            throw new InvalidOperationException($"不支持的订单状态！Status:{status}");
        }
    }
}