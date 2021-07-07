namespace IdentityServer4.MongoDB.Test
{
    using IdentityServer4.MongoDB.Database;
    using Mongo2Go;
    using System;

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
}
