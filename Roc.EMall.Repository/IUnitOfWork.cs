using System;
using System.Data.Common;

namespace Roc.EMall.Repository
{
    public interface IUnitOfWork:IDisposable
    {
        T CreateRepository<T>() where T : class,IRepository;
        void Commit();
    }
}