using System.Threading.Tasks;

namespace Roc.EMall.Repository
{
    public interface IUserQueryRepository:IQueryRepository
    {
        ValueTask<(string name, string phoneNumber, string address)> GetRecipientAsync(string userId, string recipientId);
    }
}