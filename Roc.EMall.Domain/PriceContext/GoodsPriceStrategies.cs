using System.Linq;

namespace Roc.EMall.Domain.PriceContext
{
    abstract class GoodsPriceStrategyBase : IGoodsPriceStrategy
    {
        protected GoodsPriceStrategyBase Next { get; }

        protected GoodsPriceStrategyBase(GoodsPriceStrategyBase next)
        {
            Next = next;
        }

        public abstract void InternalCalc(GoodsPriceStrategyContext context);

        public void Calc(GoodsPriceStrategyContext context)
        {
            InternalCalc(context);
            Next?.Calc(context);
        }
    }

    /// <summary>
    /// 原价策略
    /// </summary>
    class OriginPriceStrategy : GoodsPriceStrategyBase
    {
        public OriginPriceStrategy(GoodsPriceStrategyBase next) : base(next)
        {
        }

        public override void InternalCalc(GoodsPriceStrategyContext context)
        {
            var totalAmount = context.Goods.Select(it =>
            {
                it.Amount = it.Price * it.Quantity;
                return it;
            }).Sum(it => it.Amount);
            context.Amount = totalAmount;
        }
    }

    /// <summary>
    /// 总价满100元，打八折
    /// </summary>
    class DiscountPriceStrategy : GoodsPriceStrategyBase
    {
        public DiscountPriceStrategy(GoodsPriceStrategyBase next) : base(next)
        {
        }

        public override void InternalCalc(GoodsPriceStrategyContext context)
        {
            if (context.Amount >= 100)
            {
                context.Amount *= 0.8M;
            }
        }
    }
}