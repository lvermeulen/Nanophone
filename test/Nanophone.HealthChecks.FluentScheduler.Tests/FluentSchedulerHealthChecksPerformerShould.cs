using System;
using System.Threading.Tasks;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.HealthChecks.FluentScheduler.Tests
{
    public class FluentSchedulerHealthChecksPerformerShould
    {
        [Fact(Skip = "Not sure yet how FluentScheduler works")]
        public async Task ExecuteHealthChecks()
        {
            bool isExecuted = false;
            var host = new InMemoryRegistryHost
            {
                HealthChecksPerformer = new FluentSchedulerHealthChecksPerformer(() => isExecuted = true)
            };
            var checkId = await host.RegisterHealthCheckAsync(nameof(FluentSchedulerHealthChecksPerformerShould),
                nameof(FluentSchedulerHealthChecksPerformerShould), new Uri($"http://{nameof(FluentSchedulerHealthChecksPerformerShould)}"), TimeSpan.FromSeconds(1));

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.True(isExecuted);

            await host.DeregisterHealthCheckAsync(checkId);
        }
    }
}
