using Xunit;

namespace Nanophone.RegistryTenant.WebApi.Tests
{
    public class StatusControllerShould
    {
        [Fact]
        public void ReturnSuccess()
        {
            var controller = new StatusController();
            var result = controller.GetStatus();

            Assert.Equal("OK", result);
        }
    }
}
