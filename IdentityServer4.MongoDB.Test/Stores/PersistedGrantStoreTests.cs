namespace IdentityServer4.MongoDB.Test.Stores
{
    using FluentAssertions;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using IdentityServer4.MongoDB.Stores;
    using IdentityServer4.Stores;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class PersistedGrantStoreTests : IntegrationTest<OperationalStoreOptions>
    {
        private static PersistedGrant CreateTestObject(string sub = null, string clientId = null, string sid = null, string type = null)
        {
            return new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = type ?? "authorization_code",
                ClientId = clientId ?? Guid.NewGuid().ToString(),
                SubjectId = sub ?? Guid.NewGuid().ToString(),
                SessionId = sid ?? Guid.NewGuid().ToString(),
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = Guid.NewGuid().ToString()
            };
        }

        private readonly IMongoCollection<PersistedGrantEntity> _collection;

        public PersistedGrantStoreTests(MongoDatabaseFixture fixture) : base(fixture)
        {
            _collection = _database.GetCollection<PersistedGrantEntity>(_storeOptions.PersistedGrant.Name);

            if (_storeOptions.PersistedGrant.Indexes.Any())
                _collection.Indexes.CreateMany(_storeOptions.PersistedGrant.Indexes);
        }

        [Fact]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            var persistedGrant = CreateTestObject();

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            await store.StoreAsync(persistedGrant);

            var foundGrant = await _collection.AsQueryable()
                .FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);

            foundGrant.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var persistedGrant = CreateTestObject();

            await _collection.InsertOneAsync(persistedGrant.ToEntity());

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            var foundPersistedGrant = await store.GetAsync(persistedGrant.Key);

            foundPersistedGrant.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var persistedGrant = CreateTestObject();

            await _collection.InsertOneAsync(persistedGrant.ToEntity());

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            var foundPersistedGrants = (await store.GetAllAsync(new PersistedGrantFilter { SubjectId = persistedGrant.SubjectId })).ToList();

            foundPersistedGrants.Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllAsync_Should_Filter()
        {
            await _collection.InsertManyAsync(new[]
            {
                CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t1").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t2").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t1").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t2").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t1").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t2").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t1").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t2").ToEntity(),
                CreateTestObject(sub: "sub1", clientId: "c3", sid: "s3", type: "t3").ToEntity(),
                CreateTestObject().ToEntity()
            });

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());

            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1"
            })).ToList().Count.Should().Be(9);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2"
            })).ToList().Count.Should().Be(0);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1"
            })).ToList().Count.Should().Be(4);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c2"
            })).ToList().Count.Should().Be(4);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3"
            })).ToList().Count.Should().Be(1);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c4"
            })).ToList().Count.Should().Be(0);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1"
            })).ToList().Count.Should().Be(2);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3",
                SessionId = "s1"
            })).ToList().Count.Should().Be(0);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t1"
            })).ToList().Count.Should().Be(1);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t3"
            })).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            await _collection.InsertOneAsync(persistedGrant.ToEntity());

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAsync(persistedGrant.Key);

            var foundGrant = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);

            foundGrant.Should().BeNull();
        }

        [Fact]
        public async Task RemoveAllAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            await _collection.InsertOneAsync(persistedGrant.ToEntity());

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = persistedGrant.SubjectId,
                ClientId = persistedGrant.ClientId
            });

            var foundGrant = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
            foundGrant.Should().BeNull();
        }

        [Fact]
        public async Task RemoveAllAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            await _collection.InsertOneAsync(persistedGrant.ToEntity());

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = persistedGrant.SubjectId,
                ClientId = persistedGrant.ClientId,
                Type = persistedGrant.Type
            });

            var foundGrant = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
            foundGrant.Should().BeNull();
        }


        [Fact]
        public async Task RemoveAllAsync_Should_Filter()
        {
            var testObject = CreateTestObject().ToEntity();

            async Task PopulateDbAsync()
            {
                var result = await _collection.DeleteManyAsync(e => e.SubjectId == "sub1" || e.Id == testObject.Id);
                var data = new[]
                {
                    testObject,
                    CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t1").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t2").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t1").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t2").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t1").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t2").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t1").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t2").ToEntity(),
                    CreateTestObject(sub: "sub1", clientId: "c3", sid: "s3", type: "t3").ToEntity(),
               };

                await _collection.InsertManyAsync(data);
            }

            await PopulateDbAsync();
            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1"
            });
            _collection.AsQueryable().Count().Should().Be(1);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2"
            });
            _collection.AsQueryable().Count().Should().Be(10);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1"
            });
            _collection.AsQueryable().Count().Should().Be(6);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c2"
            });
            _collection.AsQueryable().Count().Should().Be(6);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3"
            });
            _collection.AsQueryable().Count().Should().Be(9);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c4"
            });
            _collection.AsQueryable().Count().Should().Be(10);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1"
            });
            _collection.AsQueryable().Count().Should().Be(8);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3",
                SessionId = "s1"
            });
            _collection.AsQueryable().Count().Should().Be(10);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t1"
            });
            _collection.AsQueryable().Count().Should().Be(9);

            await PopulateDbAsync();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t3"
            });
            _collection.AsQueryable().Count().Should().Be(10);
        }

        [Fact]
        public async Task Store_should_create_new_record_if_key_does_not_exist()
        {
            var persistedGrant = CreateTestObject();

            var foundGrant = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
            foundGrant.Should().BeNull();

            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            await store.StoreAsync(persistedGrant);

            var foundGrant2 = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
            foundGrant2.Should().NotBeNull();
        }

        [Fact]
        public async Task Store_should_update_record_if_key_already_exists()
        {
            var persistedGrant = CreateTestObject();

            await _collection.InsertOneAsync(persistedGrant.ToEntity());

            var newDate = persistedGrant.Expiration.Value.AddHours(1);
            var store = new PersistedGrantStore(_collection, FakeLogger<PersistedGrantStore>.Create());
            persistedGrant.Expiration = newDate;
            await store.StoreAsync(persistedGrant);

            var foundGrant = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
            Assert.NotNull(foundGrant);
            Assert.Equal(newDate, persistedGrant.Expiration);
        }
    }
}
