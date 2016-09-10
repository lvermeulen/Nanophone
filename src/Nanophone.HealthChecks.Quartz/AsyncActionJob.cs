using System;
using System.Threading.Tasks;
using Quartz;

namespace Nanophone.HealthChecks.Quartz
{
    public class AsyncActionJob : IJob
    {
        private const string ASYNC_ACTION = "asyncAction";

        public static JobBuilder CreateAsyncActionJob(Func<Task> asyncAction)
        {
            return JobBuilder
                .CreateForAsync<AsyncActionJob>()
                .SetJobData(new JobDataMap
                {
                    { ASYNC_ACTION, asyncAction }
                });
        }

        public Task Execute(IJobExecutionContext context)
        {
            var asyncAction = context.MergedJobDataMap[ASYNC_ACTION] as Func<Task>;
            return asyncAction?.Invoke();
        }
    }
}
