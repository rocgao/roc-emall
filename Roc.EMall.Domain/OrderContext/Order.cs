﻿using System;
using System.Collections.Generic;
using Roc.EMall.Domain.Event;

namespace Roc.EMall.Domain.OrderContext
{
    public partial class Order
    {
        private readonly string _businessId;

        private static readonly Dictionary<OrderStatus, IOrderStatusHandler> statusHandlers = new()
        {
            { OrderStatus.Submitted, new SubmittedStatusHandler() },
            { OrderStatus.Paid, new PaidStatusHandler() },
            { OrderStatus.Packaged, new PackedStatusHandler() },
        };

        public Order(long orderId, OwnerInfo owner, RecipientInfo recipient, decimal amount,OrderStatus? status, LineItem[] items)
        {
            _businessId = orderId.ToString();
            OrderId = orderId;
            Owner = owner;
            Recipient = recipient;
            Amount = amount;
            Status = status;
            Items = items;
        }

        public string BusinessId => _businessId;
        public long OrderId { get; }
        public OwnerInfo Owner { get; }
        public RecipientInfo Recipient { get; }
        public decimal Amount { get; }
        public LineItem[] Items { get; }
        public OrderStatus? Status { get; private set; }
        public long TransactionId { get; set; }

        public NewOrderEvent GetNewOrderEvent(long eventId) => new NewOrderEvent(eventId, OrderId);

        public OrderPaidEvent GetOrderPaidEvent(long eventId) => new OrderPaidEvent(eventId, OrderId);

        public void ChangeStatus(OrderStatus status)
        {
            if (statusHandlers.TryGetValue(status, out var handler))
            {
                handler.Handle(this);
                return;
            }

            throw new InvalidOperationException($"不支持的订单状态！Status:{status.ToString()}");
        }

        /// <summary>
        /// 发起付款
        /// </summary>
        /// <param name="transactionId">交易编号</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void InitiatePayment(long transactionId)
        {
            if (Status != OrderStatus.Submitted)
            {
                throw new InvalidOperationException($"订单状态不正确！Status:{Status.ToString()}");
            }

            TransactionId = transactionId;
        }

        /// <summary>
        /// 完成付款
        /// </summary>
        /// <param name="transactionId">交易编号</param>
        /// <exception cref="ArgumentException"></exception>
        public void CompletePayment(long transactionId)
        {
            if (transactionId != TransactionId)
            {
                throw new ArgumentException($"交易编号不正确！");
            }
            ChangeStatus(OrderStatus.Paid);
        }
    }
}