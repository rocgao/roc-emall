using System.Collections.Generic;

namespace Roc.EMall.Domain.PriceContext
{
    public interface IGoodsPriceStrategy
    {
        void Calc(GoodsPriceStrategyContext context);
    }
}