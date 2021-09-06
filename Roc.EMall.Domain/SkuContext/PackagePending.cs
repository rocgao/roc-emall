using System;

namespace Roc.EMall.Domain.SkuContext
{
    public record PackagePending(long OrderId, bool IsPacked);
}