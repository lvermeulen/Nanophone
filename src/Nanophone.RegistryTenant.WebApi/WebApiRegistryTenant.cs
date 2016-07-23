using System;
using Nanophone.Core;

namespace Nanophone.RegistryTenant.WebApi
{
    public class WebApiRegistryTenant : IRegistryTenant
    {
        public Uri Uri { get; }

        public WebApiRegistryTenant(Uri uri)
        {
            Uri = uri;
        }
    }
}
