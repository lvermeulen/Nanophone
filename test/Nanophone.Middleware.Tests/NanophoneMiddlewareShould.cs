using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.Middleware.Tests
{
    public class NanophoneMiddlewareShould
    {
        [Fact(Skip="Test not ready")]
        public async Task BeRegistered()
        {
            var hostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    var serviceRegistry = serviceProvider.GetRequiredService<ServiceRegistry>();
                    Assert.NotNull(serviceRegistry);
                    Assert.Equal(nameof(NanophoneMiddlewareShould), serviceRegistry.FindAllServicesAsync().Result.First().Name);
                });
                //.Configure(app =>
                //{
                //    app.UseNanophone(() => registryHost);
                //    //app.Run(async context =>
                //    //{
                //    //    await context.Response.WriteAsync("Test response");
                //    //});
                //});

            //hostBuilder.ConfigureServices(services =>
            //{
            //    var serviceProvider = services.BuildServiceProvider();
            //    var serviceRegistry = serviceProvider.GetRequiredService<ServiceRegistry>();
            //    Assert.NotNull(serviceRegistry);
            //    Assert.Equal(nameof(NanophoneMiddlewareShould), serviceRegistry.FindAllServicesAsync().Result.First().Name);
            //});

            using (var server = new TestServer(hostBuilder))
            {
            //    var request = server.CreateRequest("/");
            //    var response = await server.CreateRequest("/")
            //        .SendAsync("GET");

            //    // Assert
            //    response.EnsureSuccessStatusCode();
            //    var content = await response.Content.ReadAsStringAsync();
            //    Assert.Equal("Test response", content);
            }
        }
    }
}
