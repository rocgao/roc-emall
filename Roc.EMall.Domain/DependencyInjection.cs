using Roc.EMall.Domain.PriceContext;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            return services
                .AddTransient<IGoodsPriceStrategy>(serviceProvider =>
                {
                    var discountPriceStrategy = new DiscountPriceStrategy(null);
                    var originPriceStrategy = new OriginPriceStrategy(discountPriceStrategy);
                    return originPriceStrategy;
                });
        }
    }
}