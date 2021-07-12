namespace IdentityServer4.MongoDB.Utilities
{
    using global::MongoDB.Bson.Serialization;
    using System.Threading;
    using System;

    /// <summary>
    /// the id generator for the mongoDb entities
    /// </summary>
    internal class MongoIdGenerator : IIdGenerator
    {
        /// <summary>
        /// Generates an Id for a document.
        /// </summary>
        /// <param name="container">The container of the document (will be a MongoCollection when called from the C# driver).</param>
        /// <param name="document">The document.</param>
        /// <returns>the generated id</returns>
        public object GenerateId(object container, object document) 
            => GenerateId(document.GetType().Name);

        /// <summary>
        /// Tests whether an Id is empty.
        /// </summary>
        /// <param name="id">the id to be tested</param>
        /// <returns>true if empty, false if not</returns>
        public bool IsEmpty(object id)
            => !(id ?? "").ToString().IsValid();

        /// <summary>
        /// generate a unique id for the given type
        /// </summary>
        /// <param name="prefix">the id prefix</param>
        /// <returns>a unique generated id</returns>
        public static string GenerateId(string prefix)
            => $"{prefix}_{Generator.GenerateId()}";

        /// <summary>
        /// a partial part for <see cref="Generator"/>
        /// </summary>
        static partial class Generator
        {
            /// <summary>
            /// generate a unique tread safe id
            /// </summary>
            /// <returns>a unique string id</returns>
            public static string GenerateId() => GenerateId(Interlocked.Increment(ref _lastId));

            private const string _encode_32_Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

            private static long _lastId = DateTime.UtcNow.Ticks;

            private static readonly ThreadLocal<char[]> _buffer = new ThreadLocal<char[]>(() => new char[13]);

            private static string GenerateId(long id)
            {
                var buffer = _buffer.Value;

                buffer[0] = 'C';
                buffer[1] = _encode_32_Chars[(int)(id >> 55) & 31];
                buffer[2] = _encode_32_Chars[(int)(id >> 50) & 31];
                buffer[3] = _encode_32_Chars[(int)(id >> 45) & 31];
                buffer[4] = _encode_32_Chars[(int)(id >> 40) & 31];
                buffer[5] = _encode_32_Chars[(int)(id >> 35) & 31];
                buffer[6] = _encode_32_Chars[(int)(id >> 30) & 31];
                buffer[7] = _encode_32_Chars[(int)(id >> 25) & 31];
                buffer[8] = _encode_32_Chars[(int)(id >> 20) & 31];
                buffer[9] = _encode_32_Chars[(int)(id >> 15) & 31];
                buffer[10] = _encode_32_Chars[(int)(id >> 10) & 31];
                buffer[11] = _encode_32_Chars[(int)(id >> 5) & 31];
                buffer[12] = _encode_32_Chars[(int)id & 31];

                return new string(buffer, 0, buffer.Length);
            }
        }
    }
}
