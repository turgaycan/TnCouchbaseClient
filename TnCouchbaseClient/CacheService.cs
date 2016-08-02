using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.IO;
using Jil;
using TnCouchbaseClient.extension;

namespace TnCouchbaseClient
{
    public class CacheService : ICacheService
    {
        private static string DEFAULT_BUCKET = ConfigurationManager.AppSettings["defaultBucket"];
        private static string SERVER_DATA_URL = ConfigurationManager.AppSettings["serverUrl"];

        private const int MAX_KEY_LENGTH = 255;
        private const int MIN_KEY_LENGTH = 1;

        private static TimeSpan defaultTimeSpan = TimeSpan.FromMinutes(2);

        private IBucket bucket;

        public CacheService(string bucketName = "")
        {
            bucket = Init(bucketName);
        }

        public bool Add(string key, object value, Nullable<TimeSpan> timeToLive)
        {
            CheckKey(key);

            TimeSpan ttl = GetTimeToLive(timeToLive);

            IOperationResult<object> addOperationResult = bucket.Insert(key, value, ttl);

            bool isAdded = addOperationResult.Success;

            return isAdded;
        }

        public T Get<T>(string key)
        {
            CheckKey(key);

            IDocumentResult<T> result = bucket.GetDocument<T>(key);

            if (result.Status != ResponseStatus.Success && result.Status != ResponseStatus.KeyNotFound)
            {
                IOperationResult<T> operationResult = bucket.GetFromReplica<T>(key);

                return operationResult.Value;
            }

            T cachedValue = result.Content;

            return cachedValue;
        }

        public T GetAsValue<T>(string key)
        {
            CheckKey(key);

            IDocumentResult<T> result = bucket.GetDocument<T>(key);

            if (result.Status != ResponseStatus.Success && result.Status != ResponseStatus.KeyNotFound)
            {
                IOperationResult<T> operationResult = bucket.GetFromReplica<T>(key);

                string value = operationResult.Value as string;

                T cachedDesiliazedObject = (T)JSON.DeserializeDynamic(value);

                return cachedDesiliazedObject;
            }

            string content = result.Content as string;

            T cachedValue = (T)JSON.DeserializeDynamic(content);

            return cachedValue;
        }

        public T Put<T>(string key, object value, Nullable<TimeSpan> timeToLive)
        {
            CheckKey(key);

            TimeSpan ttl = GetTimeToLive(timeToLive);

            IOperationResult<object> result = bucket.Upsert(key, value, ttl);

            if (result.Success)
            {
                return (T)value;
            }

            return (T)result.Value;
        }

        public async Task<T> PutAsync<T>(string key, object value, Nullable<TimeSpan> timeToLive)
        {
            CheckKey(key);

            TimeSpan ttl = GetTimeToLive(timeToLive);

            IOperationResult<object> result = await bucket.UpsertAsync(key, value, ttl);

            if (result.Success)
            {
                return (T)value;
            }

            T cachedValue = (T)result.Value;

            return cachedValue;
        }

        public bool Remove(string key)
        {
            CheckKey(key);

            IOperationResult removeOperationResult = bucket.Remove(key);

            bool isRemoved = removeOperationResult.Success;

            return isRemoved;
        }

        public bool RemoveSafely(string key)
        {
            CheckKey(key);

            IOperationResult<object> operationResult = bucket.Get<object>(key);

            if (!operationResult.Success || operationResult.Value == null)
            {
                return false;
            }

            IOperationResult removeOperationResult = bucket.Remove(key);

            bool isRemoved = removeOperationResult.Success;

            return isRemoved;
        }

        public object GetAndTouch(string key, Nullable<TimeSpan> timeToLive)
        {
            CheckKey(key);

            TimeSpan ttl = GetTimeToLive(timeToLive);

            IOperationResult<object> result = bucket.GetAndTouch<object>(key, ttl);

            return result.Value;
        }

        public bool Cas(string key)
        {
            CheckKey(key);

            IOperationResult<object> casObject = bucket.Get<object>(key);

            ulong casValue = casObject.Cas;

            IOperationResult operationResult = bucket.Unlock(key, casValue);
             
            return operationResult.Status != ResponseStatus.Success;
        }

        public bool Increment(string key, Nullable<TimeSpan> timeToLive)
        {
            CheckKey(key);

            TimeSpan ttl = GetTimeToLive(timeToLive);

            IOperationResult<ulong> operationResult = bucket.Increment(key, 1, 1, ttl);

            return operationResult.Success;
        }

        private static TimeSpan GetTimeToLive(TimeSpan? timeToLive)
        {
            return (TimeSpan)((timeToLive == null || ((TimeSpan)timeToLive).TotalMilliseconds <= 1) ? defaultTimeSpan : timeToLive);
        }

        private void CheckKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length <= MIN_KEY_LENGTH || key.Length > MAX_KEY_LENGTH)
            {
                throw new ArgumentException(string.Format("Cache key : {0} length not appropriate, it must be min : {1} , max : {2}", key, MIN_KEY_LENGTH, MAX_KEY_LENGTH));
            }
        }
        
        private IBucket Init(string bucketName)
        {

            if (string.IsNullOrWhiteSpace(bucketName))
            {
                bucketName = DEFAULT_BUCKET;
            }

            ClusterHelper.Initialize(new ClientConfiguration()
            {
                Servers = new List<Uri>
                {
                    new Uri(SERVER_DATA_URL)
                },
                UseSsl = false,
                Serializer = () => new JilSerializer()
            });

            IBucket bucket = ClusterHelper.GetBucket(bucketName);

            return bucket;
        }
    }
}
