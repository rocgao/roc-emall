using System;

namespace Roc.EMall.Domain.Exception
{
    public class SkuOccupationException:ApplicationException
    {
        public SkuOccupationException(string reason) : base(reason){}
    }
}