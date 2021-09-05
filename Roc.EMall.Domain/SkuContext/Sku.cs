using System.Collections.Generic;
using Roc.EMall.Domain.Exception;

namespace Roc.EMall.Domain.SkuContext
{
    public class Sku
    {
        public long SkuId { get; }

        public Sku(long skuId)
        {
            SkuId = skuId;
            OpsRecords = new List<SkuOpsRecord>();
        }

        public string Name { get; set; }
        public int Available { get; set; }
        public int Balance { get; set; }
        public int Occupied { get; set; }
        public int Used { get; set; }
        public ICollection<SkuOpsRecord> OpsRecords { get; }

        public void Occupy(int quantity, string businessId,string @operator)
        {
            if (quantity > Available)
            {
                throw new SkuOccupationException($"库存不足。Goods:{Name}({SkuId}),Available:{Available},Occupied:{quantity}");
            }

            Available -= quantity;
            Occupied += quantity;
            OpsRecords.Add(new SkuOpsRecord(SkuId,quantity,@operator,SkuOpsKind.Occupation,businessId));
        }
    }
}