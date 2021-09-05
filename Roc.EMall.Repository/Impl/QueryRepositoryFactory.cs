using System;
using Microsoft.Extensions.DependencyInjection;

namespace Roc.EMall.Repository.Impl
{
    class QueryRepositoryFactory:IQueryRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T CreateRepository<T>() where T : IQueryRepository
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}