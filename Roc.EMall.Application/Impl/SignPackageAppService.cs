using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class SignPackageAppService : ISignPackageAppService
    {
        private readonly IPackageQueryRepository _packageQueryRepository;
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;
        private readonly IdWorker _idWorker;

        public SignPackageAppService(IPackageQueryRepository packageQueryRepository,IUOWFactory uowFactory,IDomainEventPublisher eventPublisher,IdWorker idWorker)
        {
            _packageQueryRepository = packageQueryRepository;
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
            _idWorker = idWorker;
        }

        public async ValueTask SignAsync(long packageId)
        {
            var package = await _packageQueryRepository.GetAsync(packageId)??throw new ArgumentNullException($"包裹不存在！packageId:{packageId}");
            package.Sign();
            
            // 持久化数据
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IPackageRepository>();
            await repo.StoreAsync(package);
            uow.Commit();
            
            // 发布领域事件
            await _eventPublisher.PublishAsync(package.GetSignedEvent(_idWorker.NextId()));
        }
    }
}