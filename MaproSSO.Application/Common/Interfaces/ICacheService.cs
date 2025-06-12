using System;
using System.Threading.Tasks;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> ExistsAsync(string key);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }
}