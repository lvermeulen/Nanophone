using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Nanophone.RegistryTenant.WebApi.Logging;

namespace Nanophone.RegistryTenant.WebApi
{
    public class StatusController : ApiController
    {
        private static readonly ILog s_log = LogProvider.For<StatusController>();

        [Route("status")]
        public string GetStatus()
        {
            s_log.Info("Status: OK");
            return "OK";
        }
    }
}
