using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace SampleService.Nancy
{
    public class CustomersModule : NancyModule
    {
        public CustomersModule()
        {
            Get["/"] = param => "Hello, customers";
        }
    }
}
