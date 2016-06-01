using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Common.Logging;

namespace Nanophone.RegistryTenant.WebApi
{
    public class StatusController : ApiController
    {
        private static readonly ILog s_log = LogManager.GetLogger<StatusController>();

        [Route("status")]
        public string GetStatus()
        {
            s_log.Info("Status: OK");
            return "OK";
        }
    }
}
