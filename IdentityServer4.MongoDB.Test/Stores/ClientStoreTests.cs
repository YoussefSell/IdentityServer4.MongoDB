namespace IdentityServer4.MongoDB.Test.Stores
{
    using FluentAssertions;
    using global::MongoDB.Driver;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using IdentityServer4.MongoDB.Stores;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Sdk;

    public class ClientStoreTests : IntegrationTest<ConfigurationStoreOptions>
    {
        private readonly IMongoCollection<ClientEntity> _collection;

        public ClientStoreTests(MongoDatabaseFixture fixture) : base(fixture)
        {
            _collection = _database.GetCollection<ClientEntity>(_storeOptions.Client.Name);

            if (_storeOptions.Client.Indexes.Any())
                _collection.Indexes.CreateMany(_storeOptions.Client.Indexes);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientDoesNotExist_ExpectNull()
        {
            // arrange
            var store = new ClientStore(_collection);

            // act
            var client = await store.FindClientByIdAsync(Guid.NewGuid().ToString());

            // assert
            client.Should().BeNull();
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExists_ExpectClientRetured()
        {
            // arrange
            await _collection.InsertOneAsync(new ClientEntity
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            });

            // act
            var store = new ClientStore(_collection);
            var client = await store.FindClientByIdAsync("test_client");

            // assert
            client.Should().NotBeNull();
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExistsWithCollections_ExpectClientReturnedCollections()
        {
            // arrange
            var testClient = new ClientEntity
            {
                ClientId = "properties_test_client_1",
                ClientName = "Properties Test Client",
                AllowedCorsOrigins = { "https://localhost" },
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                AllowedScopes = { "openid", "profile", "api1" },
                Claims = { new ClientClaim("test", "value") },
                ClientSecrets = { new Secret("secret".Sha256()) },
                IdentityProviderRestrictions = { "AD" },
                PostLogoutRedirectUris = { "https://locahost/signout-callback" },
                Properties = { { "foo1", "bar1" }, { "foo2", "bar2" }, },
                RedirectUris = { "https://locahost/signin" }
            };

            await _collection.InsertOneAsync(testClient);

            // act
            var store = new ClientStore(_collection);
            var client = await store.FindClientByIdAsync(testClient.ClientId);

            // assert
            client.ClientId.Should().BeEquivalentTo(testClient.ClientId);
            client.ClientName.Should().BeEquivalentTo(testClient.ClientName);
            client.AllowedCorsOrigins.Should().BeEquivalentTo(testClient.AllowedCorsOrigins);
            client.AllowedGrantTypes.Should().BeEquivalentTo(testClient.AllowedGrantTypes);
            client.AllowedScopes.Should().BeEquivalentTo(testClient.AllowedScopes);
            client.Claims.Should().BeEquivalentTo(testClient.Claims);
            client.ClientSecrets.Should().BeEquivalentTo(testClient.ClientSecrets);
            client.IdentityProviderRestrictions.Should().BeEquivalentTo(testClient.IdentityProviderRestrictions);
            client.PostLogoutRedirectUris.Should().BeEquivalentTo(testClient.PostLogoutRedirectUris);
            client.Properties.Should().BeEquivalentTo(testClient.Properties);
            client.RedirectUris.Should().BeEquivalentTo(testClient.RedirectUris);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientsExistWithManyCollections_ExpectClientReturnedInUnderFiveSeconds()
        {
            var testClient = new ClientEntity
            {
                ClientId = "test_client_with_uris",
                ClientName = "Test client with URIs",
                AllowedScopes = { "openid", "profile", "api1" },
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials
            };

            for (int i = 0; i < 50; i++)
            {
                testClient.RedirectUris.Add($"https://localhost/{i}");
                testClient.PostLogoutRedirectUris.Add($"https://localhost/{i}");
                testClient.AllowedCorsOrigins.Add($"https://localhost:{i}");
            }

            await _collection.InsertOneAsync(testClient);
            await _collection.InsertManyAsync(Enumerable.Range(0, 50)
                .Select(i => new ClientEntity
                {
                    ClientId = testClient.ClientId + i,
                    ClientName = testClient.ClientName,
                    RedirectUris = testClient.RedirectUris,
                    AllowedScopes = testClient.AllowedScopes,
                    AllowedGrantTypes = testClient.AllowedGrantTypes,
                    AllowedCorsOrigins = testClient.AllowedCorsOrigins,
                    PostLogoutRedirectUris = testClient.PostLogoutRedirectUris,
                }));

            var store = new ClientStore(_collection);

            const int timeout = 5000;
            var task = Task.Run(() => store.FindClientByIdAsync(testClient.ClientId));

            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                var client = task.Result;

                // assert
                client.ClientId.Should().BeEquivalentTo(testClient.ClientId);
                client.ClientName.Should().BeEquivalentTo(testClient.ClientName);
                client.AllowedCorsOrigins.Should().BeEquivalentTo(testClient.AllowedCorsOrigins);
                client.AllowedGrantTypes.Should().BeEquivalentTo(testClient.AllowedGrantTypes);
                client.AllowedScopes.Should().BeEquivalentTo(testClient.AllowedScopes);
                client.Claims.Should().BeEquivalentTo(testClient.Claims);
                client.ClientSecrets.Should().BeEquivalentTo(testClient.ClientSecrets);
                client.IdentityProviderRestrictions.Should().BeEquivalentTo(testClient.IdentityProviderRestrictions);
                client.PostLogoutRedirectUris.Should().BeEquivalentTo(testClient.PostLogoutRedirectUris);
                client.Properties.Should().BeEquivalentTo(testClient.Properties);
                client.RedirectUris.Should().BeEquivalentTo(testClient.RedirectUris);
            }
            else
            {
                throw new TestTimeoutException(timeout);
            }
        }
    }
}
