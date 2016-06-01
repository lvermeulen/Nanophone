using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Nancy;

namespace Nanophone.RegistryTenant.Nancy
{
    public class StatusModule : NancyModule
    {
        private static readonly ILog s_log = LogManager.GetLogger<StatusModule>();

        public StatusModule()
        {
            Get["/status"] = param =>
            {
                s_log.Info("Status: OK");
                return "OK";
            };
        }
    }
}
