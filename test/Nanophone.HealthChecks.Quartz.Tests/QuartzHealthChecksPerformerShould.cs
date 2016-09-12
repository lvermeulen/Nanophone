using System;
using System.Threading.Tasks;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.HealthChecks.Quartz.Tests
{
    public class QuartzHealthChecksPerformerShould
    {
        [Fact]
        public async Task ExecuteHealthChecks()
        {
            bool isExecuted = false;
            var host = new InMemoryRegistryHost
            {
                HealthChecksPerformer = new QuartzHealthChecksPerformer(() => isExecuted = true)
            };
            var checkId = await host.RegisterHealthCheckAsync(nameof(QuartzHealthChecksPerformerShould),
                nameof(QuartzHealthChecksPerformerShould), new Uri($"http://{nameof(QuartzHealthChecksPerformerShould)}"), TimeSpan.FromSeconds(1));

            await Task.Delay(TimeSpan.FromSeconds(2));
            Assert.True(isExecuted);

            await host.DeregisterHealthCheckAsync(checkId);
        }
    }
}
