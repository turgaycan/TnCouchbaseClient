using System;
using System.Threading.Tasks;

namespace TnCouchbaseClient
{
    public interface ICacheService
    {
        bool Add(string key, object value, Nullable<TimeSpan> timeToLive = null);

        T Get<T>(string key);

        T GetAsValue<T>(string key);

        T Put<T>(string key, object value, Nullable<TimeSpan> timeToLive = null);

        Task<T> PutAsync<T>(string key, object value, Nullable<TimeSpan> timeToLive = null);

        bool Remove(string key);

        bool RemoveSafely(string key);

        object GetAndTouch(string key, Nullable<TimeSpan> timeToLive = null);

        bool Cas(string key);

        bool Increment(string key, Nullable<TimeSpan> timeToLive);
    }
}
