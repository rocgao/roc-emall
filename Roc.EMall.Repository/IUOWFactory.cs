namespace Roc.EMall.Repository
{
    public interface IUOWFactory
    {
        IUnitOfWork Create();
    }
}