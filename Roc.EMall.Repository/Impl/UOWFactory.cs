using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Roc.EMall.Repository.Impl
{
    class UOWFactory:IUOWFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UOWFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork Create()
        {
            var uow = _serviceProvider.GetRequiredService<UnitOfWork>();
            return uow;
        }
    }
}