namespace Roc.EMall.Domain.SkuContext
{
    public enum SkuOpsKind
    {
        // 占用库存
        Occupation,
        
        // 入库 
        In,
    }
    
    /// <summary>
    /// 库存操作流水
    /// </summary>
    /// <param name="SkuId">SKU编号</param>
    /// <param name="Quantity">数量</param>
    /// <param name="Operator">操作员</param>
    /// <param name="OpsKind">操作类型</param>
    /// <param name="BusinessId">操作方业务编号</param>
    public record SkuOpsRecord(long SkuId,int Quantity,string Operator,SkuOpsKind OpsKind,string BusinessId);
}