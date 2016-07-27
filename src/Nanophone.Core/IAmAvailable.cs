using System;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IAmAvailable
    {
        Task<bool> IsAvailable();
    }

    public static class IAmAvailableExtensions
    {
        public static IAmAvailable FromUri(Uri uri)
        {
            AvailableUri isAvailable = uri;
            return isAvailable;
        }
    }
}
