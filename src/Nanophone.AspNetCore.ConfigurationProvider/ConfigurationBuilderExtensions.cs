using System;
using Microsoft.Extensions.Configuration;
using Nanophone.Core;

namespace Nanophone.AspNetCore.ConfigurationProvider
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddNanophoneKeyValues(this IConfigurationBuilder builder, Func<IRegistryHost> registryHostFactory)
        {
            builder.Add(new NanophoneConfigurationSource(registryHostFactory));
            return builder;
        }
    }
}
