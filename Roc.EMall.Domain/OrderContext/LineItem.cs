namespace Roc.EMall.Domain.OrderContext
{
    public record LineItem(long GoodsId,int Quantity,decimal Amount);
}