using Roc.EMall.Infra;

namespace Roc.EMall.Domain
{
    public abstract class AggregatedRoot:IConcurrencyVersion
    {
        public long Id { get; }

        public int ConVersion { get; set; }

        protected AggregatedRoot(long id) => Id = id;
    }
}