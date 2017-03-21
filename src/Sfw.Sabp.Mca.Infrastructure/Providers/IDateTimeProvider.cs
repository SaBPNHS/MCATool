using System;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}
