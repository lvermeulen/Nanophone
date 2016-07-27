using System;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public sealed class AvailableUri : IAmAvailable
    {
        private static Func<Task<bool>> s_isAvailable;

        private AvailableUri(Func<Task<bool>> isAvailable)
        {
            s_isAvailable = isAvailable;
        }

        public Task<bool> IsAvailable()
        {
            return s_isAvailable == null ? Task.FromResult(false) : s_isAvailable();
        }

        public static implicit operator AvailableUri(Uri uri)
        {
            if (uri == null)
            {
                s_isAvailable = () => Task.FromResult(false);
            }

            s_isAvailable = () =>
            {
                // TODO: use HttpClient to check success on GET
                //var httpClient = new HttpClient();
                //var result = await httpClient.GetAsync(uri);
                //return result.IsSuccessStatusCode;
                return Task.FromResult(false);
            };

            return new AvailableUri(s_isAvailable);
        }
    }
}
