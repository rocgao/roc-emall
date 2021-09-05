using System.Threading.Tasks;

namespace Roc.EMall.Application
{
    public interface ISubmitOrderAppService
    {
        public record Dto(string OwnerId, string RecipientId,DtoItem[] items);
        public record DtoItem(long GoodsId, int Quantity);
        ValueTask<long> SubmitAsync(Dto dto);
    }
}