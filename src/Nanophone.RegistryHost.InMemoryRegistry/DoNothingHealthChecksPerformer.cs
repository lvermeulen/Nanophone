using System.Threading.Tasks;
using Nanophone.Core;

namespace Nanophone.RegistryHost.InMemoryRegistry
{
    public class DoNothingHealthChecksPerformer : IPerformHealthChecks
    {
        public Task StartAsync()
        {
            return Task.FromResult(0);
        }

        public Task StopAsync()
        {
            return Task.FromResult(0);
        }

        public Task AddHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            return Task.FromResult(0);
        }

        public Task RemoveHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            return Task.FromResult(0);
        }

        public Task ExecuteHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            return Task.FromResult(0);
        }
    }
}
