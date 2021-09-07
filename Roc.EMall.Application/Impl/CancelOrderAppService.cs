using System;
using System.Threading.Tasks;
using Roc.EMall.Domain.Event;
using Roc.EMall.Repository;
using Snowflake.Core;

namespace Roc.EMall.Application.Impl
{
    class CancelOrderAppService:ICancelOrderAppService
    {
        private readonly IdWorker _idWorker;
        private readonly IOrderQueryRepository _orderQueryRepository;
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;

        public CancelOrderAppService(IdWorker idWorker,IOrderQueryRepository orderQueryRepository,IUOWFactory uowFactory,IDomainEventPublisher eventPublisher)
        {
            _idWorker = idWorker;
            _orderQueryRepository = orderQueryRepository;
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
        }

        public async ValueTask CancelAsync(long orderId)
        {
            var order = await _orderQueryRepository.GetAsync(orderId)??throw new ArgumentNullException($"订单不存在！orderId:{orderId}");
            order.Cancel();
            
            // 持久化数据
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            await repo.StoreAsync(order);
            uow.Commit();
            
            // 发布领域事件
            await _eventPublisher.PublishAsync(order.GetOrderCanceledEvent(_idWorker.NextId()));
        }
    }
}