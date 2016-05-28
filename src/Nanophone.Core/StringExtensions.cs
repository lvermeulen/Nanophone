using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.Core
{
    public static class StringExtensions
    {
        public static string TrimStart(this string source, string trim, StringComparison stringComparison = StringComparison.Ordinal)
        {
            string s = source;
            while (s.StartsWith(trim, stringComparison))
            {
                s = s.Substring(trim.Length - 1);
            }

            return s;
        }
    }
}
