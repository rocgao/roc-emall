namespace Roc.EMall.Domain.OrderContext
{
    internal interface IOrderStatusHandler
    {
        void Handle(Order order);
    }
}