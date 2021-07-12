namespace IdentityServer4.MongoDB.Stores
{
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.Stores;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of IResourceStore thats uses EF.
    /// </summary>
    /// <seealso cref="IResourceStore" />
    public partial class ResourceStore
    {
        /// <inheritdoc/>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null)
                throw new ArgumentNullException(nameof(apiResourceNames));

            var resources = await _apiResourceCollection.AsQueryable()
                .Where(resource => apiResourceNames.Contains(resource.Name))
                .ToListAsync();

            if (resources.Any())
            {
                Logger.LogDebug("Found {count} API resource in database", resources.Count);
            }
            else
            {
                Logger.LogDebug("Did not find any API resource in database");
            }

            return resources;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
                throw new ArgumentNullException(nameof(scopeNames));

            var resources = await _apiResourceCollection
                .Find(Builders<ApiResourceEntity>.Filter
                    .AnyIn(e => e.Scopes, scopeNames))
                .ToListAsync();

            if (resources.Any())
            {
                Logger.LogDebug("Found {count} API resource in database", resources.Count);
            }
            else
            {
                Logger.LogDebug("Did not find any API resource in database");
            }

            return resources;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
                throw new ArgumentNullException(nameof(scopeNames));

            var resources = await _apiScopeCollection.AsQueryable()
                .Where(resource => scopeNames.Contains(resource.Name))
                .ToListAsync();

            if (resources.Any())
            {
                Logger.LogDebug("Found {count} scopes in database", resources.Count);
            }
            else
            {
                Logger.LogDebug("Did not find any scopes in database");
            }

            return resources;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
                throw new ArgumentNullException(nameof(scopeNames));

            var resources = await _identityResourcesCollection.AsQueryable()
                .Where(resource => scopeNames.Contains(resource.Name))
                .ToListAsync();

            if (resources.Any())
            {
                Logger.LogDebug("Found {count} scopes in database", resources.Count);
            }
            else
            {
                Logger.LogDebug("Did not find any scopes in database");
            }

            return resources;
        }

        /// <inheritdoc/>
        public async Task<Resources> GetAllResourcesAsync()
        {
                var identityResources= await _identityResourcesCollection.AsQueryable().ToListAsync();
                var apiResources= await _apiResourceCollection.AsQueryable().ToListAsync();
                var apiScopes= await _apiScopeCollection.AsQueryable().ToListAsync();

            var resources = new Resources(
                identityResources: await _identityResourcesCollection.AsQueryable().ToListAsync(),
                apiResources: await _apiResourceCollection.AsQueryable().ToListAsync(),
                apiScopes: await _apiScopeCollection.AsQueryable().ToListAsync());

            Logger.LogDebug("Found a total of {count} scopes in database",
                resources.IdentityResources.Count +
                resources.ApiResources.Count +
                resources.ApiScopes.Count);

            return resources;
        }
    }

    /// <summary>
    /// partial part for <see cref="ResourceStore"/>
    /// </summary>
    public partial class ResourceStore : IResourceStore
    {
        /// <summary>
        /// the ApiResource mongoDb Collection.
        /// </summary>
        protected readonly IMongoCollection<ApiResourceEntity> _apiResourceCollection;

        /// <summary>
        /// the ApiScope mongoDb Collection.
        /// </summary>
        protected readonly IMongoCollection<ApiScopeEntity> _apiScopeCollection;

        /// <summary>
        /// the IdentityResource mongoDb Collection.
        /// </summary>
        protected readonly IMongoCollection<IdentityResourceEntity> _identityResourcesCollection;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<ResourceStore> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="apiScopeCollection">The apiScope mongoDb collection.</param>
        /// <param name="apiResourceCollection">The apiResource mongoDb collection.</param>
        /// <param name="identityResourcesCollection">The identityResources mongoDb collection.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">if the context is null</exception>
        public ResourceStore(
            IMongoCollection<ApiScopeEntity> apiScopeCollection,
            IMongoCollection<ApiResourceEntity> apiResourceCollection,
            IMongoCollection<IdentityResourceEntity> identityResourcesCollection,
            ILogger<ResourceStore> logger)
        {
            Logger = logger;

            _apiScopeCollection = apiScopeCollection ?? throw new ArgumentNullException(nameof(apiScopeCollection));
            _apiResourceCollection = apiResourceCollection ?? throw new ArgumentNullException(nameof(apiResourceCollection));
            _identityResourcesCollection = identityResourcesCollection ?? throw new ArgumentNullException(nameof(identityResourcesCollection));
        }
    }
}
