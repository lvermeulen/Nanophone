using System;
using Microsoft.AspNetCore.Builder;
using Nanophone.Core;

namespace Nanophone.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNanophone(this IApplicationBuilder app, Func<IRegistryHost> registryHostFactory)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (registryHostFactory == null)
            {
                throw new ArgumentNullException(nameof(registryHostFactory));
            }

            return app.UseMiddleware<NanophoneMiddleware>(registryHostFactory);
        }
    }
}
