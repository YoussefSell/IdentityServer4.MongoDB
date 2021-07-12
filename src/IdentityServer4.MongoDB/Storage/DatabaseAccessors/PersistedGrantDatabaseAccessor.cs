namespace IdentityServer4.MongoDB.Database
{
    using global::MongoDB.Bson.Serialization;
    using global::MongoDB.Driver;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using System;

    /// <summary>
    /// an accessor to the IdentityServer operational database implementation for <see cref="IPersistedGrantDatabaseAccessor"/>
    /// </summary>
    public class PersistedGrantDatabaseAccessor : IPersistedGrantDatabaseAccessor
    {
        /// <summary>
        /// create an instance of <see cref="PersistedGrantDatabaseAccessor"/>
        /// </summary>
        /// <param name="options">the operational store Options</param>
        public PersistedGrantDatabaseAccessor(OperationalStoreOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.DatabaseOptions is null)
                throw new ArgumentNullException(nameof(OperationalStoreOptions.DatabaseOptions));

            if (options.DatabaseOptions.MongoClientSettings is null)
                throw new ArgumentNullException(nameof(OperationalStoreOptions.DatabaseOptions.MongoClientSettings));

            if (string.IsNullOrEmpty(options.DatabaseOptions.DatabaseName))
                throw new ArgumentNullException(nameof(OperationalStoreOptions.DatabaseOptions.DatabaseName));

            var mongoCliant = new MongoClient(options.DatabaseOptions.MongoClientSettings);
            Database = mongoCliant.GetDatabase(options.DatabaseOptions.DatabaseName, options.DatabaseOptions.MongoDatabaseSettings);
        }

        /// <inheritdoc/>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// configure the mappings of the entities with mongoDb
        /// </summary>
        public static void ConfigureMapping()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(DeviceCodeEntity)))
            {
                BsonClassMap.RegisterClassMap<DeviceCodeEntity>(options =>
                {
                    options.AutoMap();
                    options.MapIdField(e => e.UserCode);
                });
            }
        }
    }
}
