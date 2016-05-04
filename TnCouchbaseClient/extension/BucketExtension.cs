using System;
using Couchbase;
using Couchbase.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TnCouchbaseClient.extension
{
    /// <summary>
    /// ref: https://github.com/couchbaselabs/couchbase-net-contrib/blob/master/Src/Couchbase.NetClient.Contrib/BucketExtensions.cs
    /// Not used yet
    /// </summary>
    public static class BucketExtension
    {
        /// <summary>
        /// Inserts a JSON string into the Bucket, assuming that the key is embedded within the JSON itself.
        /// </summary>
        /// <typeparam name="T">The POCO</typeparam>
        /// <param name="bucket">The <see cref="IBucket"/> to store the item</param>
        /// <param name="json">The JSON string to store.</param>
        /// <param name="fieldNameForId">The field name that is embedded within the JSON to use as the key.</param>
        /// <returns>A <see cref="IDocumentResult"/> with the result of the insert.</returns>
        public static IDocumentResult<T> ExtractKeyAndInsert<T>(this IBucket bucket, string json, string fieldNameForId)
        {
            string ignored;
            return ExtractKeyAndInsert<T>(bucket, json, fieldNameForId, out ignored);
        }

        /// <summary>
        /// Inserts a JSON string into the Bucket, assuming that the key is embedded within the JSON itself.
        /// </summary>
        /// <typeparam name="T">The POCO</typeparam>
        /// <param name="bucket">The <see cref="IBucket"/> to store the item</param>
        /// <param name="json">The JSON string to store.</param>
        /// <param name="fieldNameForId">The field name that is embedded within the JSON to use as the key.</param>
        /// <param name="id">The the key that was embedded within the document.</param>
        /// <returns>A <see cref="IDocumentResult"/> with the result of the insert.</returns>
        public static IDocumentResult<T> ExtractKeyAndInsert<T>(this IBucket bucket, string json, string fieldNameForId, out string id)
        {
            var type = typeof(T);
            var document = type == typeof(object) ?
                GetDocumentForDynamic<T>(json, fieldNameForId, out id) :
                GetDocumentForPOCO<T>(type, json, fieldNameForId, out id);

            return bucket.Insert(document);
        }

        static IDocument<T> GetDocumentForDynamic<T>(string json, string fieldNameForId, out string id)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            Document<T> document;

            JToken value = null;
            if (jObject.TryGetValue(fieldNameForId, out value))
            {
                jObject.Remove(fieldNameForId);
                document = new Document<T>
                {
                    Id = id = value.ToString(),
                    Content = JsonConvert.DeserializeObject<T>(jObject.ToString())
                };
            }
            else
            {
                throw new ArgumentException("Cannot find id field in json.", fieldNameForId);
            }
            return document;
        }

        private static IDocument<T> GetDocumentForPOCO<T>(Type type, string json, string fieldNameForId, out string id)
        {
            var document = new Document<T>
            {
                Content = JsonConvert.DeserializeObject<T>(json)
            };

            var idProperty = type.GetProperty(fieldNameForId);
            if (idProperty == null)
            {
                throw new ArgumentException("Cannot find id field in json.", fieldNameForId);
            }
            id = idProperty.GetValue(document.Content).ToString();
            document.Id = id;
            return document;
        }
    }
}
