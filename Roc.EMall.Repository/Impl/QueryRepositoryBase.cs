using System.Data;
using System.Data.Common;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;

namespace Roc.EMall.Repository.Impl
{
    abstract class QueryRepositoryBase:IQueryRepository
    {
        private readonly DbConnection _database;
        protected DbConnection Database => _database;

        protected QueryRepositoryBase(IConfiguration config,IDbConnectionFactory dbConnectionFactory)
        {
            _database = dbConnectionFactory.CreateConnection(config.GetConnectionString("RocEMallDBQuery"));
            _database.Open();
        }

        public void Dispose()
        {
            if (_database.State != ConnectionState.Closed)
            {
                _database.Close();
            }
        }
    }
}