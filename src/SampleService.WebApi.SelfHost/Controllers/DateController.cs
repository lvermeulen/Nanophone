using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SampleService.WebApi.SelfHost.Controllers
{
    public class DateController : ApiController
    {
        public DateTime GetDate()
        {
            return DateTime.UtcNow;
        }
    }
}
