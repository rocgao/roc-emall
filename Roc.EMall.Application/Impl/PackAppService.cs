using System;
using System.Linq;
using System.Threading.Tasks;
using Roc.EMall.Domain.Event;
using Roc.EMall.Domain.SkuContext;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class PackAppService:IPackAppService
    {
        private readonly IOrderQueryRepository _orderQueryRepository;
        private readonly IdWorker _idWorker;
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;

        public PackAppService(IOrderQueryRepository orderQueryRepository,IdWorker idWorker,IUOWFactory uowFactory,IDomainEventPublisher eventPublisher)
        {
            _orderQueryRepository = orderQueryRepository;
            _idWorker = idWorker;
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
        }

        public async ValueTask<long> CreateAsync(long orderId)
        {
            var order = await _orderQueryRepository.GetAsync(orderId)??throw new ArgumentNullException($"订单不存在！orderId:{orderId}");
            var package = new Package(_idWorker.NextId(), order.OrderId, order.Recipient.Name, order.Recipient.PhoneNumber, order.Recipient.Address,
                order.Items.Select(it => (it.GoodsId, it.Quantity)).ToArray());

            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IPackageRepository>();
            await repo.StoreAsync(package);
            uow.Commit();
            
            // 发布领域事件
            await _eventPublisher.PublishAsync(package.GetOrderPackedEvent(_idWorker.NextId()));

            return package.Id;
        }
    }
}