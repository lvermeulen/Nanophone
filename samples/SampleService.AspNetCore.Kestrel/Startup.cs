using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nanophone.AspNetCore.ApplicationServices;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.WebApi;
using NLog.Extensions.Logging;

namespace SampleService.AspNetCore.Kestrel
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);
            var consulConfig = new ConsulRegistryHostConfiguration { HostName = appSettings.Consul.HostName, Port = appSettings.Consul.Port };
            services.AddNanophone(() => new ConsulRegistryHost(consulConfig));
            services.AddMvc();
            services.AddOptions();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            var log = loggerFactory
                .AddNLog()
                //.AddConsole()
                //.AddDebug()
                .CreateLogger<Startup>();

            env.ConfigureNLog("NLog.config");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // add tenant & health check
            var localAddress = DnsHelper.GetIpAddressAsync().Result;
            var uri = new Uri($"http://{localAddress}:{Program.PORT}/");
            log.LogInformation("Registering tenant at ${uri}");
            var registryInformation = app.AddTenant("values", "1.7.0-pre", uri, tags: new[] {"urlprefix-/values"});
            log.LogInformation("Registering additional health check");
            var checkId = app.AddHealthCheck(registryInformation, new Uri(uri, "randomvalue"), TimeSpan.FromSeconds(15), "random value");

            // prepare checkId for options injection
            app.ApplicationServices.GetService<IOptions<HealthCheckOptions>>().Value.HealthCheckId = checkId;

            // register service & health check cleanup
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                log.LogInformation("Removing tenant & additional health check");
                app.RemoveHealthCheck(checkId);
                app.RemoveTenant(registryInformation.Id);
            });

        }
    }
}
