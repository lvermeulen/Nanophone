using Consul;
using Xunit;

namespace Nanophone.RegistryHost.ConsulRegistry.Tests
{
    public class HealthCheckExtensionsShould
    {
        [Fact]
        public void IgnoreNullHealthCheck()
        {
            Assert.False(HealthCheckExtensions.NeedsStatusCheck(null));
        }

        [Fact]
        public void IgnoreConsul()
        {
            var healthCheck = new HealthCheck { ServiceName = "consul" };
            Assert.False(healthCheck.NeedsStatusCheck());
        }

        [Fact]
        public void IgnoreServicesWithoutServiceId()
        {
            var healthCheck = new HealthCheck { ServiceID = "" };
            Assert.False(healthCheck.NeedsStatusCheck());
        }

        [Fact]
        public void IgnoreSystemHealthChecks()
        {
            var healthCheck = new HealthCheck { CheckID = "serfHealth" };
            Assert.False(healthCheck.NeedsStatusCheck());
        }

        [Fact]
        public void IgnoreServicesInNodeMaintenance()
        {
            var healthCheck = new HealthCheck { CheckID = "_node_maintenance" };
            Assert.False(healthCheck.NeedsStatusCheck());
        }

        [Fact]
        public void IgnoreServicesInMaintenance()
        {
            var healthCheck = new HealthCheck { CheckID = "_service_maintenance:" };
            Assert.False(healthCheck.NeedsStatusCheck());
        }

        [Fact]
        public void NotIgnoreNormalService()
        {
            var healthCheck = new HealthCheck
            {
                ServiceName = nameof(NotIgnoreNormalService),
                ServiceID = nameof(NotIgnoreNormalService),
                CheckID = nameof(NotIgnoreNormalService)
            };
            Assert.True(healthCheck.NeedsStatusCheck());
        }
    }
}
