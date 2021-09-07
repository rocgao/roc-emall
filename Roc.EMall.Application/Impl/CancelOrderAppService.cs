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
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;

        public CancelOrderAppService(IdWorker idWorker,IUOWFactory uowFactory,IDomainEventPublisher eventPublisher)
        {
            _idWorker = idWorker;
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
        }

        public async ValueTask CancelAsync(long orderId)
        {
            // 持久化数据
            using var uow = _uowFactory.Create();
            var repo = uow.CreateRepository<IOrderRepository>();
            
            var order = await repo.GetAsync(orderId)??throw new ArgumentNullException($"订单不存在！orderId:{orderId}");
            order.Cancel();
            await repo.StoreAsync(order);

            // 撤消库存占用
            var skuRepo = uow.CreateRepository<ISkuRepository>();
            foreach (var orderItem in order.Items)
            {
                var sku = await skuRepo.GetAsync(orderItem.GoodsId, new ISkuRepository.LoadOpsRecordOption(order.Id));
                sku.UndoOccupied(order.Id,orderItem.Quantity,"handler");

                await skuRepo.StoreAsync(sku);
            }
            
            // 提交修改
            uow.Commit();
            
            // 发布领域事件
            await _eventPublisher.PublishAsync(order.GetOrderCanceledEvent(_idWorker.NextId()));
        }
    }
}