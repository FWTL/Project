using System;
using System.Threading.Tasks;

namespace FWTL.Core.Services
{
    public interface ICacheService
    {
        void Set<T>(string key, T value, TimeSpan? expire) where T : class;

        T Get<T>(string key, T value) where T : class;

        Task<T> GetAsync<T>(string key, Func<Task<T>> fallback, TimeSpan? expire) where T : class;
    }
}