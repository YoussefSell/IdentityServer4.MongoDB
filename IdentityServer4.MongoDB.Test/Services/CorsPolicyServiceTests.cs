namespace IdentityServer4.MongoDB.Test.Services
{
    using global::MongoDB.Driver;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using IdentityServer4.MongoDB.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class CorsPolicyServiceTests : IntegrationTest<ConfigurationStoreOptions>
    {
        private readonly IMongoCollection<ClientEntity> _collection;

        public CorsPolicyServiceTests(MongoDatabaseFixture fixture) : base(fixture)
        {
            _collection = _database.GetCollection<ClientEntity>(_storeOptions.Client.Name);

            if (_storeOptions.Client.Indexes.Any())
                _collection.Indexes.CreateMany(_storeOptions.Client.Indexes);
        }

        [Fact]
        public async System.Threading.Tasks.Task IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrueAsync()
        {
            const string testCorsOrigin = "https://identityserver.io/";

            var client1 = new ClientEntity
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = Guid.NewGuid().ToString(),
                AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
            };

            var client2 = new ClientEntity
            {
                ClientId = "2",
                ClientName = "2",
                AllowedCorsOrigins = new List<string> { "https://www.identityserver.com", testCorsOrigin }
            };

            await _collection.InsertOneAsync(client1);
            await _collection.InsertOneAsync(client2);

            var service = new CorsPolicyService(_collection, FakeLogger<CorsPolicyService>.Create());
            var result = service.IsOriginAllowedAsync(testCorsOrigin).Result;

            Assert.True(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalseAsync()
        {
            await _collection.InsertOneAsync(new ClientEntity
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = Guid.NewGuid().ToString(),
                AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
            });

            var service = new CorsPolicyService(_collection, FakeLogger<CorsPolicyService>.Create());
            var result = service.IsOriginAllowedAsync("InvalidOrigin").Result;

            Assert.False(result);
        }
    }
}
