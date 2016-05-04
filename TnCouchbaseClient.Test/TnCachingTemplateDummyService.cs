using System.Collections.Generic;

namespace TnCouchbaseClient.Test
{

    public class TnCachingTemplateDummyService : ITnCachingTemplateDummyService
    {
        private readonly Dictionary<int, CouchbaseCacheModel> cacheModels = new Dictionary<int, CouchbaseCacheModel>
        {
            {1, new CouchbaseCacheModel(1, "turgay") },
            {2, new CouchbaseCacheModel(1, "turgay2") },
            {3, new CouchbaseCacheModel(1, "turgay3") },
            {4, new CouchbaseCacheModel(1, "turgay4") }
        };

        public CouchbaseCacheModel FindById(int id)
        {
            return cacheModels[id];
        }
    }
}
