using System;
using System.Collections.Generic;
using System.Linq;
#if NETSTANDARD1_6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif

namespace Nanophone.RegistryTenant.WebApi
{
    public class ApiControllerShim
#if NETSTANDARD1_6
        : Controller
#else
        : ApiController
#endif
    { }
}
