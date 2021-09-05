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
    }
}