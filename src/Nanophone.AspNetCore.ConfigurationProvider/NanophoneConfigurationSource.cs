using System;
using Microsoft.Extensions.Configuration;
using Nanophone.Core;

namespace Nanophone.AspNetCore.ConfigurationProvider
{
    public class NanophoneConfigurationSource : IConfigurationSource
    {
        private readonly Func<IRegistryHost> _registryHostFactory;

        public NanophoneConfigurationSource(Func<IRegistryHost> registryHostFactory)
        {
            _registryHostFactory = registryHostFactory;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new NanophoneConfigurationProvider(_registryHostFactory);
        }
    }
}
