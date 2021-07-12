namespace IdentityServer4.MongoDB.Test.TokenCleanup
{
    using FluentAssertions;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using IdentityServer4.MongoDB.Stores;
    using IdentityServer4.Stores;
    using IdentityServer4.Test;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class TokenCleanupTests : IntegrationTest<OperationalStoreOptions>
    {
        private readonly IMongoCollection<PersistedGrantEntity> _collection;
        private readonly IMongoCollection<DeviceCodeEntity> _deviceCodeCollection;

        public TokenCleanupTests(MongoDatabaseFixture fixture) : base(fixture)
        {
            _collection = _database.GetCollection<PersistedGrantEntity>(_storeOptions.PersistedGrant.Name);

            if (_storeOptions.PersistedGrant.Indexes.Any())
                _collection.Indexes.CreateMany(_storeOptions.PersistedGrant.Indexes);

            _deviceCodeCollection = _database.GetCollection<DeviceCodeEntity>(_storeOptions.DeviceFlowCodes.Name);

            if (_storeOptions.DeviceFlowCodes.Indexes.Any())
                _deviceCodeCollection.Indexes.CreateMany(_storeOptions.DeviceFlowCodes.Indexes);
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredGrantsExist_ExpectExpiredGrantsRemoved()
        {
            var expiredGrant = new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "app1",
                Type = "reference",
                SubjectId = "123",
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "{!}"
            };

            await _collection.InsertOneAsync(expiredGrant.ToEntity());
            await CreateSut().RemoveExpiredGrantsAsync();

            var found = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == expiredGrant.Key);
            found.Should().BeNull();
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenValidGrantsExist_ExpectValidGrantsInDb()
        {
            var validGrant = new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "app1",
                Type = "reference",
                SubjectId = "123",
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "{!}"
            };

            await _collection.InsertOneAsync(validGrant.ToEntity());
            await CreateSut().RemoveExpiredGrantsAsync();

            var found = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == validGrant.Key);
            found.Should().NotBeNull();
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredDeviceGrantsExist_ExpectExpiredDeviceGrantsRemoved()
        {
            var expiredGrant = new DeviceCodeEntity
            {
                DeviceCode = Guid.NewGuid().ToString(),
                UserCode = Guid.NewGuid().ToString(),
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "{!}"
            };

            await _deviceCodeCollection.InsertOneAsync(expiredGrant);

            await CreateSut().RemoveExpiredGrantsAsync();

            _deviceCodeCollection.AsQueryable().FirstOrDefault(x => x.DeviceCode == expiredGrant.DeviceCode).Should().BeNull();
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenValidDeviceGrantsExist_ExpectValidDeviceGrantsInDb()
        {
            var validGrant = new DeviceCodeEntity
            {
                DeviceCode = Guid.NewGuid().ToString(),
                UserCode = "2468",
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "{!}"
            };

            await _deviceCodeCollection.InsertOneAsync(validGrant);

            await CreateSut().RemoveExpiredGrantsAsync();

            _deviceCodeCollection.AsQueryable().FirstOrDefault(x => x.DeviceCode == validGrant.DeviceCode).Should().NotBeNull();
        }

        private TokenCleanupService CreateSut()
        {
            return new TokenCleanupService(_storeOptions, _collection, _deviceCodeCollection, FakeLogger<TokenCleanupService>.Create());
        }
    }
}
