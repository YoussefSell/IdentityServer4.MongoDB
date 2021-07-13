namespace Microsoft.Extensions.DependencyInjection
{
    using IdentityServer4.MongoDB;
    using IdentityServer4.MongoDB.Database;
    using IdentityServer4.MongoDB.Options;
    using System;
    using System.Linq;

    /// <summary>
    /// Extension methods to add MongoDB support to IdentityServer.
    /// </summary>
    public static class IdentityServerMongoDBBuilderExtensions
    {
        /// <summary>
        /// Adds configuration database to the DI system.
        /// </summary>
        /// <param name="services">the services collection.</param>
        /// <param name="databaseName">the name of the database to connect to.</param>
        /// <param name="connectionString">the database connection string.</param>
        /// <returns>instance of <see cref="IServiceCollection"/> to enable method chaining</returns>
        public static IServiceCollection AddConfigurationDatabase(this IServiceCollection services, string databaseName, string connectionString)
            => services.AddConfigurationDatabase(options => options.Connect(databaseName, connectionString));

        /// <summary>
        /// Adds configuration database to the DI system.
        /// </summary>
        /// <param name="services">the services collection</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns>instance of <see cref="IServiceCollection"/> to enable method chaining</returns>
        public static IServiceCollection AddConfigurationDatabase(this IServiceCollection services, Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            // initialize configuration store options and validate it
            var options = new ConfigurationStoreOptions();
            storeOptionsAction?.Invoke(options);
            services.AddSingleton(options);
            Validate(options);

            // mapping configuration
            ConfigurationDatabaseAccessor.ConfigureMapping();

            // register the database accessor
            services.Add<IConfigurationDatabaseAccessor, ConfigurationDatabaseAccessor>(options.RegistrationScope);

            // add the collections configuration
            services.AddConfigurationDBCollection(options.Client, options.RegistrationScope);
            services.AddConfigurationDBCollection(options.ApiScope, options.RegistrationScope);
            services.AddConfigurationDBCollection(options.ApiResource, options.RegistrationScope);
            services.AddConfigurationDBCollection(options.IdentityResource, options.RegistrationScope);

            return services;
        }

        /// <summary>
        /// Adds operational database to the DI system.
        /// </summary>
        /// <param name="services">the services collection</param>
        /// <param name="databaseName">the name of the database to connect to.</param>
        /// <param name="connectionString">the database connection string.</param>
        /// <returns>instance of <see cref="IServiceCollection"/> to enable method chaining</returns>
        public static IServiceCollection AddOperationalDatabase(this IServiceCollection services, string databaseName, string connectionString)
            => services.AddOperationalDatabase(options => options.Connect(databaseName, connectionString));

        /// <summary>
        /// Adds operational database to the DI system.
        /// </summary>
        /// <param name="services">the services collection</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns>instance of <see cref="IServiceCollection"/> to enable method chaining</returns>
        public static IServiceCollection AddOperationalDatabase(this IServiceCollection services, Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            // initialize configuration store options and validate it
            var storeOptions = new OperationalStoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            services.AddSingleton(storeOptions);
            Validate(storeOptions);

            // mapping configuration
            PersistedGrantDatabaseAccessor.ConfigureMapping();

            // register the database accessor
            services.Add<IPersistedGrantDatabaseAccessor, PersistedGrantDatabaseAccessor>(storeOptions.RegistrationScope);
            services.AddTransient<TokenCleanupService>();

            // add the collections configuration
            services.AddPersistedGrantDBCollection(storeOptions.DeviceFlowCodes, storeOptions.RegistrationScope);
            services.AddPersistedGrantDBCollection(storeOptions.PersistedGrant, storeOptions.RegistrationScope);

            return services;
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to the DI system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalStoreNotification<T>(this IServiceCollection services)
           where T : class, IOperationalStoreNotification
        {
            services.AddTransient<IOperationalStoreNotification, T>();
            return services;
        }

        /// <summary>
        /// validate the <see cref="ConfigurationStoreOptions "/> instance
        /// </summary>
        /// <param name="options">the options to be validated</param>
        private static void Validate(ConfigurationStoreOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.DatabaseOptions is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions));

            if (options.DatabaseOptions.MongoClientSettings is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions.MongoClientSettings));

            if (string.IsNullOrEmpty(options.DatabaseOptions.DatabaseName))
                throw new ArgumentException("you must provide a valid configuration database name", nameof(ConfigurationStoreOptions.DatabaseOptions.DatabaseName));

            if (options.DatabaseOptions is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions));

            Validate(options.Client);
            Validate(options.ApiScope);
            Validate(options.ApiResource);
            Validate(options.IdentityResource);

            static void Validate<TDocument>(CollectionConfiguration<TDocument> configuration)
            {
                if (configuration is null)
                    throw new ArgumentNullException($"the collection configuration for {nameof(TDocument)} is null");

                if (string.IsNullOrEmpty(configuration.Name))
                    throw new ArgumentException($"you must supply a valid collection name for {nameof(TDocument)}");
            }
        }

        /// <summary>
        /// validate the <see cref="ConfigurationStoreOptions "/> instance
        /// </summary>
        /// <param name="options">the options to be validated</param>
        private static void Validate(OperationalStoreOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.DatabaseOptions is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions));

            if (options.DatabaseOptions.MongoClientSettings is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions.MongoClientSettings));

            if (string.IsNullOrEmpty(options.DatabaseOptions.DatabaseName))
                throw new ArgumentException("you must provide a valid Operational database name", nameof(ConfigurationStoreOptions.DatabaseOptions.DatabaseName));

            if (options.DatabaseOptions is null)
                throw new ArgumentNullException(nameof(ConfigurationStoreOptions.DatabaseOptions));

            Validate(options.PersistedGrant);
            Validate(options.DeviceFlowCodes);

            static void Validate<TDocument>(CollectionConfiguration<TDocument> configuration)
            {
                if (configuration is null)
                    throw new ArgumentNullException($"the collection configuration for {nameof(TDocument)} is null");

                if (string.IsNullOrEmpty(configuration.Name))
                    throw new ArgumentException($"you must supply a valid collection name for {nameof(TDocument)}");
            }
        }

        /// <summary>
        /// add a mongodb collection into the Configuration database
        /// </summary>
        /// <typeparam name="TDocument">the type of the collection document</typeparam>
        /// <param name="services">the services instance</param>
        /// <param name="configuration">the collection configuration</param>
        /// <param name="scope">the registration scope</param>
        private static void AddConfigurationDBCollection<TDocument>(this IServiceCollection services, CollectionConfiguration<TDocument> configuration, RegistrationScope scope)
            => services.Add(scope, provider =>
            {
                var centralDatabaseAccessor = provider.GetService<IConfigurationDatabaseAccessor>();

                if (centralDatabaseAccessor is null)
                    throw new ArgumentNullException("the central Database Accessor is null");

                return centralDatabaseAccessor.Database.GetCollection<TDocument>(configuration.Name, configuration.Settings);
            });

        /// <summary>
        /// add a mongodb collection into the Persisted Grant database
        /// </summary>
        /// <typeparam name="TDocument">the type of the collection document</typeparam>
        /// <param name="services">the services instance</param>
        /// <param name="configuration">the collection configuration</param>
        /// <param name="scope">the registration scope</param>
        private static void AddPersistedGrantDBCollection<TDocument>(this IServiceCollection services, CollectionConfiguration<TDocument> configuration, RegistrationScope scope)
            => services.Add(scope, provider =>
            {
                var centralDatabaseAccessor = provider.GetService<IPersistedGrantDatabaseAccessor>();
                if (centralDatabaseAccessor is null)
                    throw new ArgumentNullException("the central Database Accessor is null");

                var collection = centralDatabaseAccessor.Database.GetCollection<TDocument>(configuration.Name, configuration.Settings);

                // check if we need to add indexes to the collection
                if (configuration.Indexes.Any())
                    collection.Indexes.CreateMany(configuration.Indexes);

                return collection;
            });
    }
}
