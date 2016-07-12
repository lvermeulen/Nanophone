using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;

namespace SampleService.Nancy.net451
{
    public class OrdersModule : NancyModule
    {
        public OrdersModule()
        {
            Get["/orders"] = parameters => "Hello, orders";
        }
    }
}
