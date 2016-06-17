using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.Core
{
    public static class UriExtensions
    {
        public static string GetHostAndPath(this Uri uri)
        {
            return uri.GetComponents(UriComponents.Host | UriComponents.Path, UriFormat.Unescaped);
        }
    }
}
