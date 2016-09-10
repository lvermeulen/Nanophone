using System;

namespace Nanophone.Core
{
    public class HealthCheckInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string ServiceId { get; set; }
        public Uri Uri { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
