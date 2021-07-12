namespace IdentityServer4.MongoDB.Test.Stores
{
    using global::MongoDB.Driver;
    using IdentityModel;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using IdentityServer4.MongoDB.Stores;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ApiResourceTests : IntegrationTest<ConfigurationStoreOptions>
    {
        protected readonly IMongoCollection<IdentityResourceEntity> _identityResourceEntityCollection;
        protected readonly IMongoCollection<ApiResourceEntity> _apiResourceEntityCollection;
        protected readonly IMongoCollection<ApiScopeEntity> _apiScopeEntityCollection;

        public ApiResourceTests(MongoDatabaseFixture fixture) : base(fixture)
        {
            _apiScopeEntityCollection = _database.GetCollection<ApiScopeEntity>(_storeOptions.ApiScope.Name);
            _apiResourceEntityCollection = _database.GetCollection<ApiResourceEntity>(_storeOptions.ApiResource.Name);
            _identityResourceEntityCollection = _database.GetCollection<IdentityResourceEntity>(_storeOptions.IdentityResource.Name);

            if (_storeOptions.ApiScope.Indexes.Any())
                _apiScopeEntityCollection.Indexes.CreateMany(_storeOptions.ApiScope.Indexes);

            if (_storeOptions.ApiResource.Indexes.Any())
                _apiResourceEntityCollection.Indexes.CreateMany(_storeOptions.ApiResource.Indexes);

            if (_storeOptions.IdentityResource.Indexes.Any())
                _identityResourceEntityCollection.Indexes.CreateMany(_storeOptions.IdentityResource.Indexes);
        }


        private static IdentityResource CreateIdentityTestResource()
        {
            return new IdentityResource()
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = true,
                UserClaims =
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name,
                }
            };
        }

        private static ApiResource CreateApiResourceTestResource()
        {
            return new ApiResource()
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Secret> { new Secret("secret".ToSha256()) },
                Scopes = { Guid.NewGuid().ToString() },
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }

        private static ApiScope CreateApiScopeTestResource()
        {
            return new ApiScope()
            {
                Name = Guid.NewGuid().ToString(),
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }


        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var resource = CreateApiResourceTestResource();

            await _apiResourceEntityCollection.InsertOneAsync(resource.ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var foundResource = (await store.FindApiResourcesByNameAsync(new[] { resource.Name })).SingleOrDefault();

            Assert.NotNull(foundResource);
            Assert.True(foundResource.Name == resource.Name);

            Assert.NotNull(foundResource.UserClaims);
            Assert.NotEmpty(foundResource.UserClaims);
            Assert.NotNull(foundResource.ApiSecrets);
            Assert.NotEmpty(foundResource.ApiSecrets);
            Assert.NotNull(foundResource.Scopes);
            Assert.NotEmpty(foundResource.Scopes);
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            var resource = CreateApiResourceTestResource();

            await _apiResourceEntityCollection.InsertOneAsync(resource.ToEntity());
            await _apiResourceEntityCollection.InsertOneAsync(CreateApiResourceTestResource().ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var foundResource = (await store.FindApiResourcesByNameAsync(new[] { resource.Name })).SingleOrDefault();

            Assert.NotNull(foundResource);
            Assert.True(foundResource.Name == resource.Name);

            Assert.NotNull(foundResource.UserClaims);
            Assert.NotEmpty(foundResource.UserClaims);
            Assert.NotNull(foundResource.ApiSecrets);
            Assert.NotEmpty(foundResource.ApiSecrets);
            Assert.NotNull(foundResource.Scopes);
            Assert.NotEmpty(foundResource.Scopes);
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourcesExist_ExpectResourcesReturned()
        {
            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            await _apiResourceEntityCollection.InsertOneAsync(testApiResource.ToEntity());
            await _apiScopeEntityCollection.InsertOneAsync(testApiScope.ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var resources = await store.FindApiResourcesByScopeNameAsync(new List<string>
            {
                testApiScope.Name
            });

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            await _identityResourceEntityCollection.InsertOneAsync(testIdentityResource.ToEntity());
            await _apiResourceEntityCollection.InsertOneAsync(testApiResource.ToEntity());
            await _apiScopeEntityCollection.InsertOneAsync(testApiScope.ToEntity());
            await _identityResourceEntityCollection.InsertOneAsync(CreateIdentityTestResource().ToEntity());
            await _apiResourceEntityCollection.InsertOneAsync(CreateApiResourceTestResource().ToEntity());
            await _apiScopeEntityCollection.InsertOneAsync(CreateApiScopeTestResource().ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var resources = await store.FindApiResourcesByScopeNameAsync(new[] { testApiScope.Name });

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }


        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var resource = CreateIdentityTestResource();

            await _identityResourceEntityCollection.InsertOneAsync(resource.ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
            {
                resource.Name
            }))
            .ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            var resource = CreateIdentityTestResource();

            await _identityResourceEntityCollection.InsertOneAsync(resource.ToEntity());
            await _identityResourceEntityCollection.InsertOneAsync(CreateIdentityTestResource().ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
            {
                resource.Name
            }))
            .ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }



        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var resource = CreateApiScopeTestResource();

            await _apiScopeEntityCollection.InsertOneAsync(resource.ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var resources = (await store.FindApiScopesByNameAsync(new List<string>
            {
                resource.Name
            }))
            .ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            var resource = CreateApiScopeTestResource();

            await _apiScopeEntityCollection.InsertOneAsync(resource.ToEntity());
            await _apiScopeEntityCollection.InsertOneAsync(CreateApiScopeTestResource().ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var resources = (await store.FindApiScopesByNameAsync(new List<string>
            {
                resource.Name
            }))
            .ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }

        [Fact]
        public async Task GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden()
        {
            var visibleIdentityResource = CreateIdentityTestResource();
            var visibleApiResource = CreateApiResourceTestResource();
            var visibleApiScope = CreateApiScopeTestResource();
            var hiddenIdentityResource = new IdentityResource { Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false };
            var hiddenApiResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                Scopes = { Guid.NewGuid().ToString() },
                ShowInDiscoveryDocument = false
            };
            var hiddenApiScope = new ApiScope
            {
                Name = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = false
            };

            await _identityResourceEntityCollection.InsertOneAsync(visibleIdentityResource.ToEntity());
            await _apiResourceEntityCollection.InsertOneAsync(visibleApiResource.ToEntity());
            await _apiScopeEntityCollection.InsertOneAsync(visibleApiScope.ToEntity());

            await _identityResourceEntityCollection.InsertOneAsync(hiddenIdentityResource.ToEntity());
            await _apiResourceEntityCollection.InsertOneAsync(hiddenApiResource.ToEntity());
            await _apiScopeEntityCollection.InsertOneAsync(hiddenApiScope.ToEntity());

            var store = new ResourceStore(
                _apiScopeEntityCollection,
                _apiResourceEntityCollection,
                _identityResourceEntityCollection,
                FakeLogger<ResourceStore>.Create());

            var resources = await store.GetAllResourcesAsync();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.NotEmpty(resources.ApiScopes);

            Assert.Contains(resources.IdentityResources, x => x.Name == visibleIdentityResource.Name);
            Assert.Contains(resources.IdentityResources, x => x.Name == hiddenIdentityResource.Name);

            Assert.Contains(resources.ApiResources, x => x.Name == visibleApiResource.Name);
            Assert.Contains(resources.ApiResources, x => x.Name == hiddenApiResource.Name);

            Assert.Contains(resources.ApiScopes, x => x.Name == visibleApiScope.Name);
            Assert.Contains(resources.ApiScopes, x => x.Name == hiddenApiScope.Name);
        }
    }
}
