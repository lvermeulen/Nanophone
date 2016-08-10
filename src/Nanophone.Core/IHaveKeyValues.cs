using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IHaveKeyValues
    {
        Task KeyValuePutAsync(string key, object value);
        Task<T> KeyValueGetAsync<T>(string key);
        Task KeyValueDeleteAsync(string key);
        Task KeyValueDeleteTreeAsync(string prefix);
        Task<string[]> KeyValuesGetKeysAsync(string prefix);
    }
}
