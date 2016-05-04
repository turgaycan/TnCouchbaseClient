using System;
using TnCouchbaseClient.template;

namespace TnCouchbaseClient
{
    public  class TnCachingTemplate<T> : CachingTemplate<T>
    {
        public TnCachingTemplate(ICacheService cacheService, TimeSpan cacheTtl, bool byPassCache) : base(cacheService, cacheTtl, byPassCache)
        {
        }

        public TnCachingTemplate(ICacheService cacheService, TimeSpan cacheTtl) : base(cacheService, cacheTtl)
        {
        }

    }
}
