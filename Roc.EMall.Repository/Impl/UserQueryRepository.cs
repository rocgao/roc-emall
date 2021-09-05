using System.Threading.Tasks;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;

namespace Roc.EMall.Repository.Impl
{
    class UserQueryRepository:QueryRepositoryBase, IUserQueryRepository
    {
        public ValueTask<(string name, string phoneNumber, string address)> GetRecipientAsync(string userId, string recipientId)
        {
            return new ValueTask<(string name, string phoneNumber, string address)>(("高鹏", "15921714890", "上海市长宁区天山西路18号"));
        }

        public UserQueryRepository(IConfiguration config,IDbConnectionFactory factory) : base(config,factory)
        {
        }
    }
}