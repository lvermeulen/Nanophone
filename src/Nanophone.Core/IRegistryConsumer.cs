using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.Core
{
    public interface IRegistryConsumer
    {
        Uri Uri { get; }
        Uri Start(string serviceName, string version);
    }
}
