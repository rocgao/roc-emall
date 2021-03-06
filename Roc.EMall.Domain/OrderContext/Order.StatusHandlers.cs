using System;

namespace Roc.EMall.Domain.OrderContext
{
    public partial class Order
    {
        private class SubmittedStatusHandler : IOrderStatusHandler
        {
            public void Handle(Order order)
            {
                if (order.Status.HasValue)
                {
                    if (order.Status.Value == OrderStatus.Submitted)
                    {
                        return;
                    }
                    throw new InvalidOperationException($"无法修改订单状态。{order.Status.Value} -> Submitted");
                }

                if (order.Items.Length == 0)
                {
                    throw new InvalidOperationException("订单不包含任何商品明细");
                }
                
                order.Status = OrderStatus.Submitted;
            }
        }

        private class PaidStatusHandler : IOrderStatusHandler
        {
            public void Handle(Order order)
            {
                if (order.Status is OrderStatus.Submitted or OrderStatus.Paid)
                {
                    order.Status = OrderStatus.Paid;
                }
                else
                {
                    throw new InvalidOperationException($"无法修改订单状态。{order.Status.ToString()} -> Paid");
                }
            }
        }

        private class PackedStatusHandler : IOrderStatusHandler
        {
            public void Handle(Order order)
            {
                if (order.Status is OrderStatus.Canceled)
                {
                    throw new InvalidOperationException("订单已取消");
                }

                order.Status = OrderStatus.Packaged;
            }
        }

        private class DeliveredStatusHandler : IOrderStatusHandler
        {
            public void Handle(Order order)
            {
                if (order.Status is OrderStatus.Canceled)
                {
                    throw new InvalidOperationException("订单已取消");
                }
                
                order.Status = OrderStatus.Delivered;
            }
        }
        
        private class SignedStatusHandler : IOrderStatusHandler
        {
            public void Handle(Order order)
            {
                if (order.Status is OrderStatus.Canceled)
                {
                    throw new InvalidOperationException("订单已取消");
                }
                
                order.Status = OrderStatus.Signed;
            }
        }

        private class CanceledStatusHandler : IOrderStatusHandler
        {
            public void Handle(Order order)
            {
                if (order.Status is OrderStatus.Canceled)
                {
                    return;
                }
                
                if (order.Status is null or OrderStatus.Submitted)
                {
                    order.CanceledTime = DateTime.Now;
                    order.Status = OrderStatus.Canceled;
                    return;
                }

                throw new InvalidOperationException($"该订单不支持取消。Status:{order.Status.ToString()}");
            }
        }
    }
}