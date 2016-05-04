using System;

namespace TnCouchbaseClient.template
{
    public abstract class CachingBaseTemplate
    {

        private readonly TimeSpan cacheTtl;
        private ICacheService cacheService;
        private bool byPassCache;

        public CachingBaseTemplate(ICacheService cacheService, TimeSpan cacheTtl, bool byPassCache)
        {
            this.cacheService = cacheService;
            this.cacheTtl = cacheTtl;
            this.byPassCache = byPassCache;
        }

        public CachingBaseTemplate(ICacheService cacheService, TimeSpan cacheTtl)
        {
            this.cacheService = cacheService;
            this.cacheTtl = cacheTtl;
        }

        protected void register<T>(string cacheKey, T entity)
        {
            if (entity == null)
            {
                return;
            }

            if (!byPassCache)
            {
                cacheService.Put<T>(cacheKey, entity, cacheTtl);
            }


        }

        protected T findFromCache<T>(string cacheKey)
        {
            return getCacheService().Get<T>(cacheKey);
        }

        public TimeSpan getCacheTtl()
        {
            return cacheTtl;
        }

        public ICacheService getCacheService()
        {
            return cacheService;
        }

        public bool isByPassCache()
        {
            return byPassCache;
        }

    }
}
