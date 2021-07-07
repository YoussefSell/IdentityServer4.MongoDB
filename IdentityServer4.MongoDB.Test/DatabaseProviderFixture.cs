using Mongo2Go;
using System;

namespace IdentityServer4.MongoDB.Test
{
    public class MongoDatabaseFixture : IDisposable
    {
        public MongoDbRunner Runner => _runner;
        private MongoDbRunner _runner;

        public MongoDatabaseFixture()
        {
            _runner = MongoDbRunner.Start();
        }

        public void Dispose()
        {
            _runner.Dispose();
            _runner = null;
        }
    }
}
