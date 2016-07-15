using System;
using System.Collections.Generic;
using System.Linq;
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
