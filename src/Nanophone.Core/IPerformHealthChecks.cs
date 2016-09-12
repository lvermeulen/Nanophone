using System;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IPerformHealthChecks
    {
        Task AddHealthCheckAsync(HealthCheckInformation healthCheckInformation, Action<bool> healthCheckResultAction);
        Task RemoveHealthCheckAsync(HealthCheckInformation healthCheckInformation);
        Task<bool> ExecuteHealthCheckAsync(HealthCheckInformation healthCheckInformation);
    }
}
