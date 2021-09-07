using System;
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
    internal class SubmitOrderAppService : ISubmitOrderAppService
    {
        private readonly IdWorker _idWorker;
        private readonly IGoodsPriceStrategy _calcAmountDomainService;
        private readonly IGoodsQueryRepository _goodsQueryRepository;
        private readonly IQueryRepositoryFactory _queryRepoFactory;
        private readonly IUOWFactory _uowFactory;
        private readonly IDomainEventPublisher _eventPublisher;

        public SubmitOrderAppService(IdWorker idWorker, IGoodsPriceStrategy calcAmountDomainService, IGoodsQueryRepository goodsQueryRepository,
            IQueryRepositoryFactory queryRepoFactory, IUOWFactory uowFactory, IDomainEventPublisher eventPublisher)
        {
            _idWorker = idWorker;
            _calcAmountDomainService = calcAmountDomainService;
            _goodsQueryRepository = goodsQueryRepository;
            _queryRepoFactory = queryRepoFactory;
            _uowFactory = uowFactory;
            _eventPublisher = eventPublisher;
        }

        public async ValueTask<long> SubmitAsync(ISubmitOrderAppService.Dto dto)
        {
            // 设置订单的Owner和收货人信息
            using var userQueryRepository = _queryRepoFactory.CreateRepository<IUserQueryRepository>();
            var recipient = (await userQueryRepository.GetRecipientAsync(dto.OwnerId, dto.RecipientId))
                .Map(it => new RecipientInfo(it.name, it.phoneNumber, it.address));

            // 计算订单总价和明细价格
            var goodsIsArray = dto.items.Select(it => it.GoodsId).ToArray();
            using var goodsQueryRepository = _queryRepoFactory.CreateRepository<IGoodsPriceQueryRepository>();
            var goods = await goodsQueryRepository.GetGoodsByIdAsync(goodsIsArray);
            var calculatedGoods = dto.items.Join(goods, it => it.GoodsId, it => it.GoodsId, (d, g) => new CalculatedGoodsPrice(d.GoodsId, g.Price, d.Quantity)).ToArray();
            var priceStrategyContext = new GoodsPriceStrategyContext(dto.OwnerId, calculatedGoods);
            _calcAmountDomainService.Calc(priceStrategyContext);

            // 获取Goods信息
            var goodsInfoArray = await _goodsQueryRepository.QueryAsync(goodsIsArray);
            var orderLineItems = calculatedGoods.Join(goodsInfoArray, c => c.GoodsId, g => g.goodsId,
                (c, g) => new LineItem(c.GoodsId, g.goodsName, c.Quantity, c.Amount)).ToArray();

            // 构建订单
            var order = new Order(_idWorker.NextId(), dto.OwnerId, recipient, priceStrategyContext.Amount, orderLineItems);
            order.ChangeStatus(OrderStatus.Submitted);

            // 使用事务锁定库存并保存订单
            using var uow = _uowFactory.Create();

            // 锁定库存
            var skuRepo = uow.CreateRepository<ISkuRepository>();
            foreach (var dtoItem in dto.items)
            {
                var sku = await skuRepo.GetAsync(dtoItem.GoodsId)??throw new ArgumentNullException($"库存记录不存在！skuId:{dtoItem.GoodsId}");
                sku.Occupy(order.Id, dtoItem.Quantity, dto.OwnerId);
                await skuRepo.StoreAsync(sku);
            }

            // 保存订单
            var orderRepo = uow.CreateRepository<IOrderRepository>();
            await orderRepo.StoreAsync(order);

            // 提交事务
            uow.Commit();

            // 发布领域事件
            await _eventPublisher.PublishAsync(order.GetNewOrderEvent(_idWorker.NextId()));

            return order.Id;
        }
    }
}