using System;
using System.Data.Common;
using Dapper.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Roc.EMall.Repository.Impl
{
    class UnitOfWork:IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbConnection _connection;
        private readonly DbTransaction _trans;

        private bool _isDisposed;

        public UnitOfWork(IServiceProvider serviceProvider, IConfiguration config,IDbConnectionFactory dbConnectionFactory)
        {
            _serviceProvider = serviceProvider;
            var connStr = config.GetConnectionString("RocEMallDB");
             _connection = dbConnectionFactory.CreateConnection(connStr);
             _connection.Open();
             _trans= _connection.BeginTransaction();
        }
        
        public T CreateRepository<T>() where T : class,IRepository
        {
            var repo = _serviceProvider.GetRequiredService<T>();
            if (repo is RepositoryBase b)
            {
                b.Database = _connection;
                b.Transaction = _trans;
            }

            return repo;
        }

        public void Commit()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("_trans");
            }
            _isDisposed = true;
            _trans.Commit();
            _connection.Close();
        }
        
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _trans.Rollback();
            _connection.Close();
        }
    }
}