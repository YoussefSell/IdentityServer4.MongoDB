namespace IdentityServer4.MongoDB.Database
{
    using global::MongoDB.Driver;
    using IdentityServer4.MongoDB.Options;
    using System;

    /// <summary>
    /// an accessor to the IdentityServer configuration database implementation for <see cref="IConfigurationDatabaseAccessor"/>
    /// </summary>
    public class ConfigurationDatabaseAccessor : IConfigurationDatabaseAccessor
    {
        /// <summary>
        /// create an instance of <see cref="ConfigurationDatabaseAccessor"/>
        /// </summary>
        /// <param name="options">the configuration store database options</param>
        public ConfigurationDatabaseAccessor(ConfigurationStoreOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.DatabaseOptions is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions));

            if (options.DatabaseOptions.MongoClientSettings is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions.MongoClientSettings));

            if (string.IsNullOrEmpty(options.DatabaseOptions.DatabaseName))
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions.DatabaseName));

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

        }
    }
}
