using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nanophone.Core;

namespace SampleService.AspNetCore.Kestrel.Controllers
{
    [Route("/randomvalue")]
    public class RandomValueController : Controller
    {
        private static readonly Random s_random = new Random(Guid.NewGuid().GetHashCode());
        private readonly HealthCheckOptions _options;
        private readonly ILogger<RandomValueController> _logger;

        public RandomValueController(IOptions<HealthCheckOptions> options, ILogger<RandomValueController> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        [HttpGet]
        public string GetRandomValue()
        {
            int random = s_random.Next(-1, 100) + 1;
            _logger.LogTrace("Random value: ${random}");
            string key = $"values/{_options.HealthCheckId}/randomvalue";

            var serviceRegistry = HttpContext.RequestServices.GetRequiredService<ServiceRegistry>();
            serviceRegistry.KeyValuePutAsync(key, random.ToString())
                .Wait();

            return "OK";
        }
    }
}
