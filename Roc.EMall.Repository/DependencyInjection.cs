using Dapper.Logging;
using MySql.Data.MySqlClient;
using Roc.EMall.Repository;
using Roc.EMall.Repository.Impl;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services.AddDbConnectionFactory((sp,connString)=>new MySqlConnection(connString), lifetime:ServiceLifetime.Singleton)
                .AddSingleton<IUOWFactory, UOWFactory>().AddTransient<UnitOfWork>()
                .AddSingleton<IQueryRepositoryFactory,QueryRepositoryFactory>()
                .AddTransient<ISkuRepository,SkuRepository>()
                .AddTransient<IGoodsPriceQueryRepository,GoodsPriceQueryRepository>()
                .AddTransient<IUserQueryRepository,UserQueryRepository>()
                .AddTransient<IOrderRepository,OrderRepository>()
                .AddTransient<IOrderQueryRepository,OrderQueryRepository>()
                .AddTransient<ITransactionRepository,TransactionRepository>()
                .AddTransient<ITransactionQueryRepository,TransactionQueryRepository>()
                .AddTransient<IPackageRepository,PackageRepository>()
                .AddTransient<IPackageQueryRepository,PackageQueryRepository>();
            
        }
    }
}