using System;
using System.Collections.Generic;
using Roc.EMall.Domain.Event;

namespace Roc.EMall.Domain.OrderContext
{
    public partial class Order
    {
        private static readonly Dictionary<OrderStatus, IOrderStatusHandler> statusHandlers = new()
        {
            { OrderStatus.Submitted, new SubmittedStatusHandler() },
            { OrderStatus.Paid, new PaidStatusHandler() },
            { OrderStatus.Packaged, new PackedStatusHandler() },
            { OrderStatus.Delivered, new DeliveredStatusHandler() },
            { OrderStatus.Signed, new SignedStatusHandler() },
            { OrderStatus.Canceled, new CanceledStatusHandler() },
        };

        public Order(long orderId, string owner, RecipientInfo recipient, decimal amount, LineItem[] items,
            OrderStatus? status = null, PaymentInfo payment = null, ExpressInfo express = null, DateTime? canceledTime = null)
        {
            OrderId = orderId;
            Owner = owner;
            Recipient = recipient;
            Amount = amount;
            Status = status;
            Express = express ?? new ExpressInfo(null, string.Empty);
            Items = items;
            Payment = payment ?? new PaymentInfo(null, null);
            CanceledTime = canceledTime;
        }

        public long OrderId { get; }
        public string Owner { get; }
        public RecipientInfo Recipient { get; }
        public decimal Amount { get; }
        public LineItem[] Items { get; }
        public OrderStatus? Status { get; private set; }
        public ExpressInfo Express { get; private set; }
        public PaymentInfo Payment { get; private set; }
        public DateTime? CanceledTime { get; private set; }

        public NewOrderEvent GetNewOrderEvent(long eventId) => new NewOrderEvent(eventId, OrderId);

        public OrderPaidEvent GetOrderPaidEvent(long eventId) => new OrderPaidEvent(eventId, OrderId);

        public OrderCanceledEvent GetOrderCanceledEvent(long eventId) => new OrderCanceledEvent(eventId, OrderId);

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

            Payment = Payment with { TransactionId = transactionId };
        }

        /// <summary>
        /// 完成付款
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void CompletePayment(DateTime paidTime)
        {
            ChangeStatus(OrderStatus.Paid);
            Payment = Payment with { PaidTime = paidTime };
        }

        public void Pack(long packageId)
        {
            ChangeStatus(OrderStatus.Packaged);
            Express = Express with { PackageId = packageId };
        }

        public void Deliver(string expressNo)
        {
            ChangeStatus(OrderStatus.Delivered);
            Express = Express with { ExpressNo = expressNo };
        }

        public void Sign()
        {
            ChangeStatus(OrderStatus.Signed);
        }

        public void Cancel()
        {
            ChangeStatus(OrderStatus.Canceled);
        }
    }
}