using System.Threading.Tasks;
using Nancy;
using Nancy.Testing;
using Xunit;

namespace Nanophone.RegistryTenant.Nancy.Tests
{
    public class StatusModuleShould
    {
        [Fact]
        public async Task ReturnSuccessAsync()
        {
            var browser = new Browser(new DefaultNancyBootstrapper());
            var result = await browser.Get("/status", with => {
                with.HttpRequest();
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("OK", result.Body.AsString());
        }
    }
}
