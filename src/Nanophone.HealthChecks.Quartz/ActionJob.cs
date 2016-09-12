using System;
using System.Threading.Tasks;
using Quartz;

namespace Nanophone.HealthChecks.Quartz
{
    public class ActionJob : IJob
    {
        private const string ACTION = "Action";

        public static JobBuilder CreateActionJob(Action action)
        {
            return JobBuilder
                .Create<ActionJob>()
                .SetJobData(new JobDataMap
                {
                    { ACTION, action }
                });
        }

        public Task Execute(IJobExecutionContext context)
        {
            var action = context.MergedJobDataMap[ACTION] as Action;
            action?.Invoke();

            return Task.FromResult(0);
        }
    }
}
