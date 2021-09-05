namespace Roc.EMall.Domain.PriceContext
{
    public record GoodsPriceStrategyContext(string OwnerId,CalculatedGoodsPrice[] Goods)
    {
        public decimal Amount { get; set; }
    }
    
    public class CalculatedGoodsPrice:GoodsPrice
    {
        public CalculatedGoodsPrice(long goodsId,decimal price, int quantity) : base(goodsId,price)
        {
            Quantity = quantity;
        }
        
        public decimal Amount { get; set; }
        public int Quantity { get; }
    }
}