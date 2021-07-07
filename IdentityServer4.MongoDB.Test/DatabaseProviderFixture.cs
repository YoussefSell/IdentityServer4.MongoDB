namespace IdentityServer4.MongoDB.Test
{
    using IdentityServer4.MongoDB.Database;
    using Mongo2Go;
    using System;
    using Xunit;

    public class MongoDatabaseFixture : IDisposable
    {
        public MongoDbRunner Runner => _runner;
        private MongoDbRunner _runner;

        public MongoDatabaseFixture()
        {
            _runner = MongoDbRunner.Start();

            PersistedGrantDatabaseAccessor.ConfigureMapping();
            ConfigurationDatabaseAccessor.ConfigureMapping();
        }

        public void Dispose()
        {
            _runner.Dispose();
            _runner = null;
        }
    }

    [CollectionDefinition("MongoDbCollection")]
    public class DatabaseCollection : ICollectionFixture<MongoDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
