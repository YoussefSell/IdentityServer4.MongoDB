namespace IdentityServer4.MongoDB.Test.Stores
{
    using FluentAssertions;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using IdentityModel;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using IdentityServer4.MongoDB.Stores;
    using IdentityServer4.Stores.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    public class DeviceFlowStoreTests : IntegrationTest<OperationalStoreOptions>
    {
        private readonly IPersistentGrantSerializer serializer = new PersistentGrantSerializer();
        private readonly IMongoCollection<DeviceCodeEntity> _collection;

        public DeviceFlowStoreTests(MongoDatabaseFixture fixture) : base(fixture)
        {
            _collection = _database.GetCollection<DeviceCodeEntity>(_storeOptions.DeviceFlowCodes.Name);

            if (_storeOptions.DeviceFlowCodes.Indexes.Any())
                _collection.Indexes.CreateMany(_storeOptions.DeviceFlowCodes.Indexes);
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenSuccessful_ExpectDeviceCodeAndUserCodeStored()
        {
            // arrange
            var deviceCode = Guid.NewGuid().ToString();
            var userCode = Guid.NewGuid().ToString();
            var data = new DeviceCode
            {
                ClientId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            await store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);

            // act
            var foundDeviceFlowCodes = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

            // assert
            foundDeviceFlowCodes.Should().NotBeNull();
            foundDeviceFlowCodes?.DeviceCode.Should().Be(deviceCode);
            foundDeviceFlowCodes?.UserCode.Should().Be(userCode);
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenSuccessful_ExpectDataStored()
        {
            var deviceCode = Guid.NewGuid().ToString();
            var userCode = Guid.NewGuid().ToString();
            var data = new DeviceCode
            {
                ClientId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            await store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);

            var foundDeviceFlowCodes = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

            foundDeviceFlowCodes.Should().NotBeNull();
            var deserializedData = new PersistentGrantSerializer().Deserialize<DeviceCode>(foundDeviceFlowCodes?.Data);

            deserializedData.CreationTime.Should().BeCloseTo(data.CreationTime);
            deserializedData.ClientId.Should().Be(data.ClientId);
            deserializedData.Lifetime.Should().Be(data.Lifetime);
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenUserCodeAlreadyExists_ExpectException()
        {
            var existingUserCode = $"user_{Guid.NewGuid()}";
            var deviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim> { new Claim(JwtClaimTypes.Subject, $"sub_{Guid.NewGuid()}") }))
            };

            await _collection.InsertOneAsync(new DeviceCodeEntity
            {
                DeviceCode = $"device_{Guid.NewGuid()}",
                UserCode = existingUserCode,
                ClientId = deviceCodeData.ClientId,
                SubjectId = deviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = deviceCodeData.CreationTime,
                Expiration = deviceCodeData.CreationTime.AddSeconds(deviceCodeData.Lifetime),
                Data = serializer.Serialize(deviceCodeData)
            });

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());

            await Assert.ThrowsAsync<MongoWriteException>(() =>
                store.StoreDeviceAuthorizationAsync($"device_{Guid.NewGuid()}", existingUserCode, deviceCodeData));
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenDeviceCodeAlreadyExists_ExpectException()
        {
            var existingDeviceCode = $"device_{Guid.NewGuid()}";
            var deviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim> { new Claim(JwtClaimTypes.Subject, $"sub_{Guid.NewGuid()}") }))
            };

            await _collection.InsertOneAsync(new DeviceCodeEntity
            {
                DeviceCode = existingDeviceCode,
                UserCode = $"user_{Guid.NewGuid()}",
                ClientId = deviceCodeData.ClientId,
                SubjectId = deviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = deviceCodeData.CreationTime,
                Expiration = deviceCodeData.CreationTime.AddSeconds(deviceCodeData.Lifetime),
                Data = serializer.Serialize(deviceCodeData)
            });

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());

            await Assert.ThrowsAsync<MongoWriteException>(() =>
                store.StoreDeviceAuthorizationAsync(existingDeviceCode, $"user_{Guid.NewGuid()}", deviceCodeData));
        }

        [Fact]
        public async Task FindByUserCodeAsync_WhenUserCodeExists_ExpectDataRetrievedCorrectly()
        {
            var testDeviceCode = $"device_{Guid.NewGuid()}";
            var testUserCode = $"user_{Guid.NewGuid()}";

            var expectedSubject = $"sub_{Guid.NewGuid()}";
            var expectedDeviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(JwtClaimTypes.Subject, expectedSubject) }))
            };

            await _collection.InsertOneAsync(new DeviceCodeEntity
            {
                DeviceCode = testDeviceCode,
                UserCode = testUserCode,
                ClientId = expectedDeviceCodeData.ClientId,
                SubjectId = expectedDeviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = expectedDeviceCodeData.CreationTime,
                Expiration = expectedDeviceCodeData.CreationTime.AddSeconds(expectedDeviceCodeData.Lifetime),
                Data = serializer.Serialize(expectedDeviceCodeData)
            });

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByUserCodeAsync(testUserCode);

            code.Should().BeEquivalentTo(expectedDeviceCodeData,
                assertionOptions => assertionOptions.Excluding(x => x.Subject));

            code.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject).Should().NotBeNull();
        }

        [Fact]
        public async Task FindByUserCodeAsync_WhenUserCodeDoesNotExist_ExpectNull()
        {
            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByUserCodeAsync($"user_{Guid.NewGuid()}");
            code.Should().BeNull();
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_WhenDeviceCodeExists_ExpectDataRetrievedCorrectly()
        {
            var testDeviceCode = $"device_{Guid.NewGuid()}";
            var testUserCode = $"user_{Guid.NewGuid()}";

            var expectedSubject = $"sub_{Guid.NewGuid()}";
            var expectedDeviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(JwtClaimTypes.Subject, expectedSubject) }))
            };

            await _collection.InsertOneAsync(new DeviceCodeEntity
            {
                DeviceCode = testDeviceCode,
                UserCode = testUserCode,
                ClientId = expectedDeviceCodeData.ClientId,
                SubjectId = expectedDeviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = expectedDeviceCodeData.CreationTime,
                Expiration = expectedDeviceCodeData.CreationTime.AddSeconds(expectedDeviceCodeData.Lifetime),
                Data = serializer.Serialize(expectedDeviceCodeData)
            });

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByDeviceCodeAsync(testDeviceCode);

            code.Should().BeEquivalentTo(expectedDeviceCodeData,
                assertionOptions => assertionOptions.Excluding(x => x.Subject)
                .Excluding(e => e.CreationTime));

            code.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject).Should().NotBeNull();
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_WhenDeviceCodeDoesNotExist_ExpectNull()
        {
            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByDeviceCodeAsync($"device_{Guid.NewGuid()}");
            code.Should().BeNull();
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_WhenDeviceCodeAuthorized_ExpectSubjectAndDataUpdated()
        {
            var testDeviceCode = $"device_{Guid.NewGuid()}";
            var testUserCode = $"user_{Guid.NewGuid()}";

            var expectedSubject = $"sub_{Guid.NewGuid()}";
            var unauthorizedDeviceCode = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true
            };

            await _collection.InsertOneAsync(new DeviceCodeEntity
            {
                DeviceCode = testDeviceCode,
                UserCode = testUserCode,
                ClientId = unauthorizedDeviceCode.ClientId,
                CreationTime = unauthorizedDeviceCode.CreationTime,
                Expiration = unauthorizedDeviceCode.CreationTime.AddSeconds(unauthorizedDeviceCode.Lifetime),
                Data = serializer.Serialize(unauthorizedDeviceCode)
            });

            var authorizedDeviceCode = new DeviceCode
            {
                ClientId = unauthorizedDeviceCode.ClientId,
                RequestedScopes = unauthorizedDeviceCode.RequestedScopes,
                AuthorizedScopes = unauthorizedDeviceCode.RequestedScopes,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(JwtClaimTypes.Subject, expectedSubject) })),
                IsAuthorized = true,
                IsOpenId = true,
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300
            };

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            await store.UpdateByUserCodeAsync(testUserCode, authorizedDeviceCode);

            var updatedCodes = await _collection.AsQueryable().SingleAsync(x => x.UserCode == testUserCode);

            // should be unchanged
            updatedCodes.DeviceCode.Should().Be(testDeviceCode);
            updatedCodes.ClientId.Should().Be(unauthorizedDeviceCode.ClientId);
            // updatedCodes.CreationTime.Should().Be(unauthorizedDeviceCode.CreationTime);
            //updatedCodes.Expiration.Should().Be(unauthorizedDeviceCode.CreationTime.AddSeconds(authorizedDeviceCode.Lifetime));

            // should be changed
            var parsedCode = serializer.Deserialize<DeviceCode>(updatedCodes.Data);
            parsedCode.Should().BeEquivalentTo(authorizedDeviceCode, assertionOptions =>
                assertionOptions.Excluding(x => x.Subject)
                .Excluding(e => e.CreationTime));

            parsedCode.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject).Should().NotBeNull();
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_WhenDeviceCodeExists_ExpectDeviceCodeDeleted()
        {
            var testDeviceCode = $"device_{Guid.NewGuid()}";
            var testUserCode = $"user_{Guid.NewGuid()}";

            var existingDeviceCode = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true
            };

            await _collection.InsertOneAsync(new DeviceCodeEntity
            {
                DeviceCode = testDeviceCode,
                UserCode = testUserCode,
                ClientId = existingDeviceCode.ClientId,
                CreationTime = existingDeviceCode.CreationTime,
                Expiration = existingDeviceCode.CreationTime.AddSeconds(existingDeviceCode.Lifetime),
                Data = serializer.Serialize(existingDeviceCode)
            });

            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            await store.RemoveByDeviceCodeAsync(testDeviceCode);

            _collection.AsQueryable().FirstOrDefault(x => x.UserCode == testUserCode).Should().BeNull();
        }
        [Fact]
        public async Task RemoveByDeviceCodeAsync_WhenDeviceCodeDoesNotExists_ExpectSuccess()
        {
            var store = new DeviceFlowStore(_collection, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());
            await store.RemoveByDeviceCodeAsync($"device_{Guid.NewGuid()}");
        }
    }
}
