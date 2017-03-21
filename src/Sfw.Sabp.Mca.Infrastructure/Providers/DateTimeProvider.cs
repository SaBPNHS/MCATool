using System;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get { return DateTime.Now; } }

        public DateTime UtcNow { get { return DateTime.UtcNow; } }
    }
}
