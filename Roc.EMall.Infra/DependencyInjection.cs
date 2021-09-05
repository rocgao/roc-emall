using Snowflake.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfra(this IServiceCollection services)
        {
            return services.AddSingleton<IdWorker>(new IdWorker(1, 1));
        }
    }
}