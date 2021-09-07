using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Roc.EMall.Domain.Exception;
using Roc.EMall.Infra;

namespace Roc.EMall.Domain.SkuContext
{
    public class Sku:AggregatedRoot
    {
        public Sku(long id, string name,
            int available,int balance,int occupied,int used,
            IEnumerable<SkuOpsRecord> opsRecords=null):base(id)
        {
            Name = name;
            Available = available;
            Balance = balance;
            Occupied = occupied;
            Used = used;
            OpsRecords =opsRecords==null? new RepositoryList<SkuOpsRecord>():new RepositoryList<SkuOpsRecord>(opsRecords);
        }

        public string Name { get; }
        public int Available { get; private set; }
        public int Balance { get; private set; }
        public int Occupied { get; private set; }
        public int Used { get; private set; }
        public RepositoryList<SkuOpsRecord> OpsRecords { get; }
        
        public void Occupy(long orderId, int quantity,string @operator)
        {
            if (quantity > Available)
            {
                throw new SkuOccupationException($"库存不足。Goods:{Name}({Id}),Available:{Available},Occupied:{quantity}");
            }

            Available -= quantity;
            Occupied += quantity;
            OpsRecords.Add(new SkuOpsRecord(Id,orderId, quantity,@operator,SkuOpsKind.Occupied));
        }

        public void Use(long orderId,int quantity, string @operator)
        {
            if (OpsRecords.Any(it => it.OpsKind == SkuOpsKind.Used))
            {
                return;
            }
            
            if (quantity > Balance)
            {
                throw new SkuOccupationException($"库存不足。Goods:{Name}({Id}),Available:{Available},Occupied:{quantity}");
            }

            Balance -= quantity;
            Occupied -= quantity;
            Used += quantity;
            OpsRecords.Add(new SkuOpsRecord(Id,orderId,quantity,@operator,SkuOpsKind.Used));
        }

        public void UndoOccupied(long orderId, int quantity, string @operator)
        {
            if (OpsRecords.Any(it => it.OpsKind == SkuOpsKind.Canceled))
            {
                return;
            }
            
            Available += quantity;
            Occupied -= quantity;
            OpsRecords.Add(new SkuOpsRecord(Id,orderId,quantity,@operator,SkuOpsKind.Canceled));
        }
    }
}