using System.Linq;
using System.Threading.Tasks;
using Roc.EMall.Domain.Event;
using Roc.EMall.Domain.Exception;
using Roc.EMall.Domain.OrderContext;
using Roc.EMall.Domain.PriceContext;
using Roc.EMall.Repository;
using Snowflake.Core;
using Roc.EMall.Infra;

namespace Roc.EMall.Application.Impl
{
    internal class SubmitOrderAppService:ISubmitOrderAppService
    {
        private readonly IdWorker _idWorker;
        private readonly IGoodsPriceStrategy _calcAmountDomainService;
        private readonly IQueryRepositoryFactory _queryRepoFactory;
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;

        public SubmitOrderAppService(IdWorker idWorker,IGoodsPriceStrategy calcAmountDomainService,
            IQueryRepositoryFactory queryRepoFactory,IUOWFactory uowFactory,IDomainEventPublisher eventPublisher)
        {
            _idWorker = idWorker;
            _calcAmountDomainService = calcAmountDomainService;
            _queryRepoFactory = queryRepoFactory;
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
        }

        public async ValueTask<long> SubmitAsync(ISubmitOrderAppService.Dto dto)
        {
            // 设置订单的Owner和收货人信息
            using var userQueryRepository = _queryRepoFactory.CreateRepository<IUserQueryRepository>();
            var owner = new OwnerInfo(dto.OwnerId);
            var recipient = (await userQueryRepository.GetRecipientAsync(dto.OwnerId, dto.RecipientId))
                .Map(it => new RecipientInfo(it.name, it.phoneNumber, it.address));
            
            // 计算订单总价和明细价格
            using var goodsQueryRepository = _queryRepoFactory.CreateRepository<IGoodsPriceQueryRepository>();
            var goods = await goodsQueryRepository.GetGoodsByIdAsync(dto.items.Select(it => it.GoodsId).ToArray());
            var calculatedGoods= dto.items.Join(goods, it => it.GoodsId, it => it.GoodsId, (d, g) => new CalculatedGoodsPrice(d.GoodsId, g.Price, d.Quantity)).ToArray();
            var priceStrategyContext = new GoodsPriceStrategyContext(dto.OwnerId, calculatedGoods);
            _calcAmountDomainService.Calc(priceStrategyContext);
            
            // 构建订单
            var order =new Order(_idWorker.NextId(),owner,recipient,priceStrategyContext.Amount,null,calculatedGoods.Select(it=>new LineItem(it.GoodsId,it.Quantity,it.Amount)).ToArray());
            order.ChangeStatus(OrderStatus.Submitted);
            
            // 使用事务锁定库存并保存订单
            using var uow = _uowFactory.Create();

            // 锁定库存
            var skuRepo= uow.CreateRepository<ISkuRepository>();
            foreach (var dtoItem in dto.items)
            {
                var success=await skuRepo.OccupyAsync(dtoItem.GoodsId, dtoItem.Quantity, order.BusinessId, dto.OwnerId);
                if (!success)
                {
                    throw new SkuOccupationException($"锁定库存失败！SkuId:{dtoItem.GoodsId} Quantity:{dtoItem.Quantity}");
                }
            }
            
            // 保存订单
            var orderRepo = uow.CreateRepository<IOrderRepository>();
            await orderRepo.StoreAsync(order);
            
            // 提交事务
            uow.Commit();
            
            // 发布领域事件
            await _eventPublisher.PublishAsync(order.GetNewOrderEvent(_idWorker.NextId()));

            return order.OrderId;
        }
    }
}