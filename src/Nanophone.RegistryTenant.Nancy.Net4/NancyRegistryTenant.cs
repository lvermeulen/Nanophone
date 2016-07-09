using System;
using System.Collections.Generic;
using System.Linq;
using Nanophone.Core;

namespace Nanophone.RegistryTenant.Nancy
{
    public class NancyRegistryTenant : IRegistryTenant
    {
        public Uri Uri { get; }

        public NancyRegistryTenant(Uri uri)
        {
            Uri = uri;
        }
    }
}
