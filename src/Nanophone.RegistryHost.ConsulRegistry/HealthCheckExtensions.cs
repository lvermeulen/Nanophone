using Consul;
using System;
using Nanophone.RegistryHost.ConsulRegistry.Logging;

namespace Nanophone.RegistryHost.ConsulRegistry
{
    public static class HealthCheckExtensions
    {
        private static readonly ILog s_log = LogProvider.For<ConsulRegistryHost>();

        public static bool NeedsStatusCheck(this HealthCheck healthCheck)
        {
            if (healthCheck == null)
            {
                return false;
            }

            string serviceName = string.IsNullOrWhiteSpace(healthCheck.ServiceName) ? "" : $" {healthCheck.ServiceName}";

            // don't check services without service ID
            if (healthCheck.ServiceID == "")
            {
                s_log.Debug($"Not checking service${serviceName}: service is missing service ID");
                return false;
            }

            if (healthCheck.CheckID.Equals("serfHealth", StringComparison.OrdinalIgnoreCase))
            {
                s_log.Debug($"Not checking service${serviceName}: service is system health check");
                return false;
            }

            if (healthCheck.CheckID.Equals("_node_maintenance", StringComparison.OrdinalIgnoreCase))
            {
                s_log.Debug($"Not checking service${serviceName}: service node is in maintenance");
                return false;
            }

            if (healthCheck.CheckID.StartsWith("_service_maintenance:", StringComparison.OrdinalIgnoreCase))
            {
                s_log.Debug($"Not checking service${serviceName}: service is in maintenance");
                return false;
            }

            return true;
        }
    }
}
