namespace Roc.EMall.Domain.OrderContext
{
    public enum OrderStatus
    {
        /// <summary>
        /// 已提交
        /// </summary>
        Submitted,

        /// <summary>
        /// 已支付
        /// </summary>
        Paid,
        
        /// <summary>
        /// 打包完成
        /// </summary>
        Packaged,
        
        /// <summary>
        /// 已发货
        /// </summary>
        Sent,
        
        /// <summary>
        /// 已签收
        /// </summary>
        Signed,
        
        /// <summary>
        /// 已取消
        /// </summary>
        Canceled,
    }
}