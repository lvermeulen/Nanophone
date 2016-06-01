using System;
using System.Collections.Generic;
using System.Linq;
using Nanophone.Core;

namespace Nanophone.RegistryConsumer.WebApi
{
    public class WebApiRegistryConsumer : IRegistryTenant
    {
        public Uri Uri { get; }

        public WebApiRegistryConsumer(Uri uri)
        {
            Uri = uri;
        }
    }
}
