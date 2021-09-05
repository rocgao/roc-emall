namespace Roc.EMall.Repository
{
    public interface IQueryRepositoryFactory
    {
        T CreateRepository<T>() where T : IQueryRepository;
    }
}