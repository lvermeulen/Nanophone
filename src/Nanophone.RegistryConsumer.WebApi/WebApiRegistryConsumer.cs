using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Nanophone.Core;

namespace Nanophone.RegistryConsumer.WebApi
{
    public class WebApiRegistryConsumer : IRegistryConsumer
    {
        public Uri Start(string serviceName, string version)
        {
            var uri = DnsHelper.GetNewLocalUri();
            var selfHostConfiguration = new HttpSelfHostConfiguration(uri);

            selfHostConfiguration.Routes.MapHttpRoute(
                "API Default", "{controller}/{id}",
                new { id = RouteParameter.Optional }
            );

            var server = new HttpSelfHostServer(selfHostConfiguration);
            server.OpenAsync().Wait();
            return uri;
        }
    }
}
