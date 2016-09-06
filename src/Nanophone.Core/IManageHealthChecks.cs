using System;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IManageHealthChecks
    {
        Task<string> RegisterHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null);
        Task<bool> DeregisterHealthCheckAsync(string checkId);
    }
}
