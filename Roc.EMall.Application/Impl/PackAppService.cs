using System;
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
        private readonly IPackageQueryRepository _packageQueryRepository;
        private readonly IdWorker _idWorker;
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;

        public PackAppService(IOrderQueryRepository orderQueryRepository,IPackageQueryRepository packageQueryRepository, IdWorker idWorker,IUOWFactory uowFactory,IDomainEventPublisher eventPublisher)
        {
            _orderQueryRepository = orderQueryRepository;
            _packageQueryRepository = packageQueryRepository;
            _idWorker = idWorker;
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
        }

        public async ValueTask<long> CreateAsync(long orderId)
        {
            var existingPackage = await _packageQueryRepository.GetByOrderIdAsync(orderId);
            if (null != existingPackage)
            {
                return existingPackage.Id;
            }
            
            var order = await _orderQueryRepository.GetAsync(orderId)??throw new ArgumentNullException($"订单不存在！orderId:{orderId}");

            var packageId = _idWorker.NextId();
            var lineItems = new PackageLineItem[order.Items.Length];
            for (var i = 0; i < order.Items.Length; i++)
            {
                var item = order.Items[i];
                lineItems[i] = new PackageLineItem(packageId, i + 1, item.GoodsId, item.GoodsName, item.Quantity);
            }
            var package = new Package(packageId, order.OrderId, order.Recipient.Name, order.Recipient.PhoneNumber, order.Recipient.Address,lineItems);

            // 持久化数据
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