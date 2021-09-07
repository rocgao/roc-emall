namespace Roc.EMall.Domain.SkuContext
{
    public enum SkuOpsKind
    {
        // 占用库存
        Occupied,
        
        // 已使用的
        Used,
        
        // 入库 
        In,
    }
    
    /// <summary>
    /// 库存操作流水
    /// </summary>
    /// <param name="SkuId">SKU编号</param>
    /// <param name="OrderId">订单Id</param>
    /// <param name="Quantity">数量</param>
    /// <param name="Operator">操作员</param>
    /// <param name="OpsKind">操作类型</param>
    public record SkuOpsRecord(long SkuId,long OrderId, int Quantity,string Operator,SkuOpsKind OpsKind);
}