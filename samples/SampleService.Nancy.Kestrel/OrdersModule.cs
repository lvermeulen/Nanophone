using Nancy;

namespace SampleService.Nancy.Kestrel
{
    public class OrdersModule : NancyModule
    {
        public OrdersModule()
        {
            Get("/orders", parameters => "Hello, orders");
        }
    }
}
