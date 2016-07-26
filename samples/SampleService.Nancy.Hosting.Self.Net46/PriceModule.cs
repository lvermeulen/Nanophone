using Nancy;

namespace SampleService.Nancy.Hosting.Self.Net46
{
    public class PriceModule : NancyModule
    {
        public PriceModule()
        {
            Get("/price", parameters => "Hello, price");
        }
    }
}
