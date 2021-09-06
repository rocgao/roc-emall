using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class DeliverPackageAppService:IDeliverPackageAppService
    {
        private readonly IUOWFactory _uowFactory;
        private readonly IPackageQueryRepository _packageQueryRepository;
        private readonly IDomainEventPublisher _eventPublisher;
        private readonly IdWorker _idworker;

        public DeliverPackageAppService(IUOWFactory uowFactory,IPackageQueryRepository packageQueryRepository,IDomainEventPublisher eventPublisher,IdWorker idworker)
        {
            _uowFactory = uowFactory;
            _packageQueryRepository = packageQueryRepository;
            _eventPublisher = eventPublisher;
            _idworker = idworker;
        }

        public async ValueTask DeliverAsync(long packageId, string expressNo)
        {
            var package = await _packageQueryRepository.GetAsync(packageId)??throw new ArgumentNullException($"包裹不存在！packageId:{packageId}");
            
            package.Deliver(expressNo);

            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IPackageRepository>();
            await repo.StoreAsync(package);
            uow.Commit();
            
            // 发布领域事件
            await _eventPublisher.PublishAsync(package.GetDeliveredEvent(_idworker.NextId()));
        }
    }
}