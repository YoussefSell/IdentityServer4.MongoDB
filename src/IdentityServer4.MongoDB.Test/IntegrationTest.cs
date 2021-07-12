namespace IdentityServer4.MongoDB
{
    using global::MongoDB.Driver;
    using IdentityServer4.MongoDB.Options;
    using IdentityServer4.MongoDB.Test;
    using System;
    using Xunit;

    /// <summary>
    /// Base class for integration tests, responsible for initializing test database providers & an xUnit class fixture
    /// </summary>
    [Collection("MongoDbCollection")]
    public class IntegrationTest<TStoreOption>
        where TStoreOption : BaseStoreOptions
    {
        protected readonly TStoreOption _storeOptions = Activator.CreateInstance<TStoreOption>();
        protected readonly MongoDatabaseFixture _fixture;
        protected readonly IMongoDatabase _database;

        public IntegrationTest(MongoDatabaseFixture fixture)
        {
            _fixture = fixture;

            var databaseName = string.Empty;

            if (typeof(TStoreOption) == typeof(ConfigurationStoreOptions))
                databaseName = "configuration_database";

            if (typeof(TStoreOption) == typeof(OperationalStoreOptions))
                databaseName = "operational_database";

            var _client = new MongoClient(fixture.Runner.ConnectionString);
            _client.DropDatabase(databaseName);
            _database = _client.GetDatabase(databaseName);
        }
    }
}