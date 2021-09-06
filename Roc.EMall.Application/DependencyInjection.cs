using System.Reflection;
using MediatR;
using Roc.EMall.Application;
using Roc.EMall.Application.Components;
using Roc.EMall.Application.Impl;
using Roc.EMall.Domain.Event;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            return services
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddSingleton<IDomainEventPublisher,DomainEventPublisher>()
                .AddSingleton<ISubmitOrderAppService, SubmitOrderAppService>()
                .AddSingleton<IPayOrderAppService,PayOrderAppService>();
        }
    }
}