using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nanophone.Core;
using Quartz;
using Quartz.Impl;

namespace Nanophone.HealthChecks.Quartz
{
    public class QuartzHealthChecksPerformer : IPerformHealthChecks
    {
        private readonly Func<Task> _executeHealthCheck;
        private readonly Lazy<HttpClient> _httpClient;
        private readonly IScheduler _scheduler;

        public QuartzHealthChecksPerformer(Func<Task> executeHealthCheck = null)
        {
            _executeHealthCheck = executeHealthCheck;
            _httpClient = new Lazy<HttpClient>();

            var factory = new StdSchedulerFactory();
            _scheduler = factory.GetScheduler()
                .Result;
            _scheduler.Start()
                .Wait();
        }

        public async Task AddHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            var job = AsyncActionJob.CreateAsyncActionJob(_executeHealthCheck ?? (() => ExecuteHealthCheckAsync(healthCheckInformation)))
                .WithIdentity(healthCheckInformation.Id)
                .UsingJobData(nameof(HealthCheckInformation.Uri), healthCheckInformation.Uri.ToString())
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(healthCheckInformation.Id)
                .WithSimpleSchedule(x => x
                    .WithInterval(healthCheckInformation.Interval)
                    .RepeatForever())
                .StartNow()
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }

        public async Task RemoveHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            await _scheduler.DeleteJob(new JobKey(healthCheckInformation.Id));
        }

        public async Task<bool> ExecuteHealthCheckAsync(HealthCheckInformation healthCheckInformation)
        {
            var response = await _httpClient.Value.GetAsync(healthCheckInformation.Uri);

            return response.IsSuccessStatusCode;
        }
    }
}
