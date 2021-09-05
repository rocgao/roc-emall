using System.Data.Common;

namespace Roc.EMall.Repository.Impl
{
    abstract class RepositoryBase
    {
        public DbConnection Database { get; set; }
        public DbTransaction Transaction { get; set; }
    }
}