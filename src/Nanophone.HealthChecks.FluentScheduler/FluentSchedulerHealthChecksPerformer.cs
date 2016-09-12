using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentScheduler;
using Nanophone.Core;

namespace Nanophone.HealthChecks.FluentScheduler
{
    public class FluentSchedulerHealthChecksPerformer : IPerformHealthChecks
    {
        private readonly Action _executeHealthCheck;
        private readonly Lazy<HttpClient> _httpClient;
        private readonly Registry _registry = new Registry();

        public FluentSchedulerHealthChecksPerformer(Action executeHealthCheck = null)
        {
            _executeHealthCheck = executeHealthCheck;
            _httpClient = new Lazy<HttpClient>();
            JobManager.Initialize(_registry);
            JobManager.JobException += ex => System.Diagnostics.Debug.WriteLine(ex.Exception.Message);
        }

        public Task AddHealthCheckAsync(HealthCheckInformation healthCheckInformation, Action<bool> healthCheckResultAction)
        {
            if (healthCheckResultAction == null)
            {
                throw new ArgumentNullException(nameof(healthCheckResultAction));
            }

            _registry.Schedule(_executeHealthCheck ?? (() => healthCheckResultAction(ExecuteHealthCheckAsync(healthCheckInformation).Result)))
                .WithName(healthCheckInformation.Id)
                .NonReentrant()
                .ToRunNow()
                .AndEvery(Math.Max(1, (int)healthCheckInformation.Interval.TotalSeconds)).Seconds();

            return Task.FromResult(0);
        }

        public Task RemoveHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            JobManager.RemoveJob(healthCheckInformation.Id);
            return Task.FromResult(0);
        }

        public async Task<bool> ExecuteHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            var response = await _httpClient.Value.GetAsync(healthCheckInformation.Uri);

            return response.IsSuccessStatusCode;
        }
    }
}
