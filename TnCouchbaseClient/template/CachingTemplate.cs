using System;

namespace TnCouchbaseClient.template
{
    public abstract class CachingTemplate<T> : CachingBaseTemplate
    {
        public CachingTemplate(ICacheService cacheService, TimeSpan cacheTtl, bool byPassCache) : base(cacheService, cacheTtl, byPassCache)
        {

        }

        public CachingTemplate(ICacheService cacheService, TimeSpan cacheTtl) : base(cacheService, cacheTtl)
        {
        }

        /// <summary>
        /// parameterless method call
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public T findBy(string cacheKey, Func<T> query)
        {
            if (!isByPassCache())
            {
                T fromCache = findFromCache<T>(cacheKey);

                if (fromCache != null)
                {
                    return fromCache;
                }
            }

            T entity = query();

            register(cacheKey, entity);

            return entity;
        }


        /// <summary>
        /// an input type of parameter or 
        /// your input parameters more of three, use of this method pass your method parameters as model object
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T findBy<I>(string cacheKey, Func<I, T> query, I param)
        {
            if (!isByPassCache())
            {
                T fromCache = findFromCache<T>(cacheKey);

                if (fromCache != null)
                {
                    return fromCache;
                }
            }

            T entity = query(param);

            register(cacheKey, entity);

            return entity;
        }

        /// <summary>
        /// two input parameters
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="I2"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public T findBy<I, I2>(string cacheKey, Func<I, I2, T> query, I param, I2 param2)
        {
            if (!isByPassCache())
            {
                T fromCache = findFromCache<T>(cacheKey);

                if (fromCache != null)
                {
                    return fromCache;
                }
            }

            T entity = query(param, param2);

            register(cacheKey, entity);

            return entity;
        }


        /// <summary>
        /// three input  parameters
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="I2"></typeparam>
        /// <typeparam name="I3"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <returns></returns>
        public T findBy<I, I2, I3>(string cacheKey, Func<I, I2, I3, T> query, I param, I2 param2, I3 param3)
        {
            if (!isByPassCache())
            {
                T fromCache = findFromCache<T>(cacheKey);

                if (fromCache != null)
                {
                    return fromCache;
                }
            }

            T entity = query(param, param2, param3);

            register(cacheKey, entity);

            return entity;
        }

    }
}
