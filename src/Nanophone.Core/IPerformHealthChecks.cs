using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IPerformHealthChecks
    {
        Task AddHealthCheckAsync(HealthCheckInformation healthCheckInformation);
        Task RemoveHealthCheckAsync(HealthCheckInformation healthCheckInformation);
        Task ExecuteHealthCheckAsync(HealthCheckInformation healthCheckInformation);
    }
}
