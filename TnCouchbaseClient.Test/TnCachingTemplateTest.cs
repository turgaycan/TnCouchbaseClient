using System;
using Moq;
using Xunit;

namespace TnCouchbaseClient.Test
{
    public class TnCachingTemplateTest : IDisposable
    {
        private static readonly ICacheService cacheService = new CacheService();
        private static readonly Mock<ITnCachingTemplateDummyService> mockService = new Mock<ITnCachingTemplateDummyService>(MockBehavior.Strict);

        [Fact]
        public void should_get_object_by_using_template()
        {
            CouchbaseCacheModel model = new CouchbaseCacheModel(1, "turgay");

            cacheService.Add("turgay", model);

            CouchbaseCacheModel cachedModel = new TnCachingTemplate<CouchbaseCacheModel>(cacheService, TimeSpan.FromMinutes(2))
                .findBy("turgay", mockService.Object.FindById, 1);

            mockService.Verify(each => each.FindById(1), Times.Never);

            Assert.Equal(cachedModel.Id, model.Id);
            Assert.Equal(cachedModel.Name, model.Name);
        }

        [Fact]
        public void should_put_object_by_using_template()
        {
            CouchbaseCacheModel model = new CouchbaseCacheModel(1, "turgay");

            mockService.Setup(each => each.FindById(1)).Returns(model);

            CouchbaseCacheModel cachedModel = new TnCachingTemplate<CouchbaseCacheModel>(cacheService, TimeSpan.FromMinutes(2))
                .findBy("turgay", mockService.Object.FindById, 1);

            mockService.Verify(each => each.FindById(1), Times.AtMostOnce);

            Assert.Equal(cachedModel.Id, model.Id);
            Assert.Equal(cachedModel.Name, model.Name);
        }

        public void Dispose()
        {
            cacheService.RemoveSafely("turgay");
        }
    }
}
