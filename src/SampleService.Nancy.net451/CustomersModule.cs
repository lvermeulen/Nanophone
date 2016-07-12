using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;

namespace SampleService.Nancy.net451
{
    public class CustomersModule : NancyModule
    {
        public CustomersModule()
        {
            Get["/"] = (parameters, cancellationToken) => Task.FromResult("Hello, customers");
        }
    }
}
