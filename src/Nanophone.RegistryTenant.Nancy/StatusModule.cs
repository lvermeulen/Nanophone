using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nanophone.RegistryTenant.Nancy.Logging;

namespace Nanophone.RegistryTenant.Nancy
{
    public class StatusModule : NancyModule
    {
        private static readonly ILog s_log = LogProvider.For<StatusModule>();

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
