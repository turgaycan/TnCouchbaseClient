using System;

namespace TnCouchbaseClient.Test
{
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
}