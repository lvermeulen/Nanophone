using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.Core
{
    public interface IRegistryConsumer
    {
        Uri Start(string serviceName, string version);
    }
}
