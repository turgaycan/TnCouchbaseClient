using System;
using System.IO;
using Couchbase.Core.Serialization;
using Jil;

namespace TnCouchbaseClient.extension
{
    /// <summary>
    /// A custom <see cref="ITypeSerializer"/> for the Couchbase .NET SDK that uses Jil for serialization.
    /// </summary>
    /// <remarks>https://github.com/kevin-montrose/Jil</remarks>
    public class JilSerializer : ITypeSerializer
    {
        private readonly Options _options;

        public JilSerializer() : this(new Options())
        {
        }

        public JilSerializer(Options options)
        {
            _options = options;
        }

        /// <summary>
        /// Deserializes the specified stream into the <see cref="T:System.Type" /> T specified as a generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="T:System.Type" /> specified as the type of the value.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// The <see cref="T:System.Type" /> instance representing the value of the key.
        /// </returns>
        public T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return JSON.Deserialize<T>(reader, _options);
            }
        }

        /// <summary>
        /// Deserializes the specified buffer into the <see cref="T:System.Type" /> T specified as a generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="T:System.Type" /> specified as the type of the value.</typeparam>
        /// <param name="buffer">The buffer to deserialize from.</param>
        /// <param name="offset">The offset of the buffer to start reading from.</param>
        /// <param name="length">The length of the buffer to read from.</param>
        /// <returns>
        /// The <see cref="T:System.Type" /> instance representing the value of the key.
        /// </returns>
        public T Deserialize<T>(byte[] buffer, int offset, int length)
        {
            T value = default(T);
            if (length == 0) return value;
            using (var ms = new MemoryStream(buffer, offset, length))
            {
                using (var sr = new StreamReader(ms))
                {
                    //use the following code block only for value types
                    //strangely enough Nullable<T> itself is a value type so we need to filter it out
                    if (typeof(T).IsValueType && (!typeof(T).IsGenericType
                        || typeof(T).GetGenericTypeDefinition() != typeof(Nullable<>)))
                    {
                        //we can't declare Nullable<T> because T is not restricted to struct in this method scope
                        var type = typeof(Nullable<>).MakeGenericType(typeof(T));
                        object nullableVal = JSON.Deserialize(sr, type, _options);
                        //either we have a null or an instance of Nullabte<T> that can be cast directly to T
                        value = nullableVal == null ? default(T) : (T)nullableVal;
                    }
                    else
                    {
                        value = JSON.Deserialize<T>(sr, _options);
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Serializes the specified object into a buffer.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>
        /// A <see cref="T:System.Byte" /> array that is the serialized value of the key.
        /// </returns>
        public byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    JSON.Serialize(obj, sw, _options);
                }
                return ms.ToArray();
            }
        }
    }
}
