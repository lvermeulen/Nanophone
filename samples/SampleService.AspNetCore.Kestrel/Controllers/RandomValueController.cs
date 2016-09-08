using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nanophone.Core;

namespace SampleService.AspNetCore.Kestrel.Controllers
{
    [Route("/randomvalue")]
    public class RandomValueController : Controller
    {
        private static readonly Random s_random = new Random(Guid.NewGuid().GetHashCode());
        private readonly HealthCheckOptions _options;

        public RandomValueController(IOptions<HealthCheckOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        public string GetRandomValue()
        {
            int random = s_random.Next(-1, 100) + 1;
            string key = $"values/{_options.HealthCheckId}/randomvalue";

            var serviceRegistry = HttpContext.RequestServices.GetRequiredService<ServiceRegistry>();
            serviceRegistry.KeyValuePutAsync(key, random.ToString());

            return "OK";
        }
    }
}
