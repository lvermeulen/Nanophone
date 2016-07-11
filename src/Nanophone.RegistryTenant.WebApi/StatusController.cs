using Nanophone.RegistryTenant.WebApi.Logging;
#if NETSTANDARD1_6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif

namespace Nanophone.RegistryTenant.WebApi
{
    public class StatusController : ApiControllerShim
    {
        private static readonly ILog s_log = LogProvider.For<StatusController>();

        [Route("/status")]
        public string GetStatus()
        {
            s_log.Info("Status: OK");
            return "OK";
        }
    }
}
