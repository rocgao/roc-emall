namespace Roc.EMall.Domain.OrderContext
{
    public record LineItem(long GoodsId,string GoodsName,int Quantity,decimal Amount);
}