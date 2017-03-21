
# TnCouchbaseClient

.NET Couchbase caching template... 1.0.0
Simple way of Couchbase Caching Operations and Caching Template

The goal of project is to reduce code duplication, to make easier of code maintenance and make central of caching operations

---

# Getting Started

Download from github repo : https://github.com/TurgayCan2/TnCouchbaseClient

To install .NET Couchbase caching template library for doing caching operations, run the following command in the Package Manager Console

# Install-Package TnCouchbaseClient

# Prerequisites/Dependencies

	.NETFramework 4.5

	CouchbaseNetClient (>= 2.4.2)
	Newtonsoft.Json (>= 10.0.1)
	Jil (>= 2.15.0)
	Sigil (>= 4.7.0)
	log4net (>= 2.0.8)
	Common.Logging (>= 3.3.1)
	Common.Logging.Core (>= 3.3.1)
	Version History
	Version	Downloads	Last updated	Listed
	.NET Couchbase... 1.0.0 (this version)	0

# Code Examples

You can downlaod couchbase cache server for developer/community/enterprise edition from -> https://www.couchbase.com/downloads

Download from nuget package manager or code source repository and build/export library as dll.

Add project as a reference with avobe dependencies.

Add couchbase cache server config ;

  <appSettings>
    <add key="serverUrl" value="http://127.0.0.1:8091/pools"/>
    <add key="defaultBucket" value="default"/>
  </appSettings>

Create a dummy model;

    [Serializable]
    public class CouchbaseCacheModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CouchbaseCacheModel()
        {
        }

        public CouchbaseCacheModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
	

create a dummy service for caching ;

    public interface ITnCachingTemplateDummyService
    {
        CouchbaseCacheModel FindById(int id);
    }

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
	
	
test implementation of dummy model/service;

I used xunit for unit/integration tests;

add xunit to project as nuget manager -> Install-Package xunit


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
	
	
# Versioning (Current 1.0.0)

Download from github repo : https://github.com/TurgayCan2/TnCouchbaseClient

# Authors

Turgay Can

# Tags
caching template .net couchbase
