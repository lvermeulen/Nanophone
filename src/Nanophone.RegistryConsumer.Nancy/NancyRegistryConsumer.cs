using System;
using System.Collections.Generic;
using System.Linq;
using Nanophone.Core;

namespace Nanophone.RegistryConsumer.Nancy
{
    public class NancyRegistryConsumer : IRegistryConsumer
    {
        public Uri Uri { get; }

        public NancyRegistryConsumer(Uri uri)
        {
            Uri = uri;
        }
    }
}
