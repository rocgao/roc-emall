namespace Roc.EMall.Domain.PriceContext
{
    public class GoodsPrice
    {
        public GoodsPrice(long goodsId,decimal price)
        {
            GoodsId = goodsId;
            Price = price;
        }
        
        public long GoodsId { get; }
        public decimal Price { get; }
    }
}