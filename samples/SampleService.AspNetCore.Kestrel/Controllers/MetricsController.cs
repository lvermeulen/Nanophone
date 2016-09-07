using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nanophone.Core;

namespace SampleService.AspNetCore.Kestrel.Controllers
{
    [Route("/metrics")]
    public class MetricsController : Controller
    {
        private static readonly Random s_random = new Random(Guid.NewGuid().GetHashCode());
        private readonly HealthCheckOptions _options;

        public MetricsController(IOptions<HealthCheckOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        public string GetMetrics()
        {
            int random = s_random.Next(-1, 100) + 1;

            var serviceRegistry = HttpContext.RequestServices.GetRequiredService<ServiceRegistry>();
            string key = $"values/{_options.HealthCheckId}/metrics/cpu-usage";
            serviceRegistry.KeyValuePutAsync(key, random.ToString());

            return "OK";
        }
    }
}
