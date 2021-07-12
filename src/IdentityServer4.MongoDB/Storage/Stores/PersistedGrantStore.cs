namespace IdentityServer4.MongoDB.Stores
{
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.Stores;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// the persisted grant store
    /// </summary>
    public partial class PersistedGrantStore
    {
        /// <inheritdoc/>
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var persistedGrants = await Filter(_collection.AsQueryable(), filter)
                .ToListAsync();

            Logger.LogDebug("{persistedGrantCount} persisted grants found for {@filter}", persistedGrants.Count, filter);

            return persistedGrants;
        }

        /// <inheritdoc/>
        public async Task<PersistedGrant> GetAsync(string key)
        {
            var grant = await _collection.AsQueryable().SingleOrDefaultAsync(grantInDb => grantInDb.Key == key);

            Logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, !(grant is null));

            return grant;
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var filters = new List<FilterDefinition<PersistedGrantEntity>>();

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                filters.Add(Builders<PersistedGrantEntity>.Filter.Where(x => x.ClientId == filter.ClientId));
            }
            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                filters.Add(Builders<PersistedGrantEntity>.Filter.Where(x => x.SessionId == filter.SessionId));
            }
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                filters.Add(Builders<PersistedGrantEntity>.Filter.Where(x => x.SubjectId == filter.SubjectId));
            }
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                filters.Add(Builders<PersistedGrantEntity>.Filter.Where(x => x.Type == filter.Type));
            }

            await _collection.DeleteManyAsync(Builders<PersistedGrantEntity>.Filter
                .And(filters));
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            var persistedGrant = await _collection.AsQueryable().SingleOrDefaultAsync(x => x.Key == key);
            if (persistedGrant is null)
            {
                Logger.LogDebug("no {persistedGrantKey} persisted grant found in database", key);
                return;
            }

            Logger.LogDebug("removing {persistedGrantKey} persisted grant from database", key);
            var deleteResult = await _collection.DeleteManyAsync(grant => grant.Key == key);
            //if (deleteResult.)
        }

        /// <inheritdoc/>
        public async Task StoreAsync(PersistedGrant grant)
        {
            var existing = await _collection.AsQueryable().SingleOrDefaultAsync(grantInDB => grantInDB.Key == grant.Key);
            if (existing is null)
            {
                Logger.LogDebug("{persistedGrantKey} not found in database", grant.Key);
                await _collection.InsertOneAsync(PersistedGrantEntity.MapFrom(grant));
            }
            else
            {
                Logger.LogDebug("{persistedGrantKey} found in database", grant.Key);

                var updateResult = await _collection.UpdateOneAsync(grantInDB => grantInDB.Key == grant.Key,
                    update: Builders<PersistedGrantEntity>.Update
                    .Set(grantInDB => grantInDB.Type, grant.Type)
                    .Set(grantInDB => grantInDB.Data, grant.Data)
                    .Set(grantInDB => grantInDB.ClientId, grant.ClientId)
                    .Set(grantInDB => grantInDB.SubjectId, grant.SubjectId)
                    .Set(grantInDB => grantInDB.SessionId, grant.SessionId)
                    .Set(grantInDB => grantInDB.Expiration, grant.Expiration)
                    .Set(grantInDB => grantInDB.Description, grant.Description)
                    .Set(grantInDB => grantInDB.ConsumedTime, grant.ConsumedTime)
                    .Set(grantInDB => grantInDB.CreationTime, grant.CreationTime));

                //if(updateResult.)
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="PersistedGrantStore"/>
    /// </summary>
    public partial class PersistedGrantStore : IPersistedGrantStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IMongoCollection<PersistedGrantEntity> _collection;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="collection">The PersistedGrant collection.</param>
        /// <param name="logger">The logger.</param>
        public PersistedGrantStore(
            IMongoCollection<PersistedGrantEntity> collection,
            ILogger<PersistedGrantStore> logger)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            Logger = logger;
        }

        private static IMongoQueryable<PersistedGrantEntity> Filter(IMongoQueryable<PersistedGrantEntity> query, PersistedGrantFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                query = query.Where(x => x.ClientId == filter.ClientId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                query = query.Where(x => x.SessionId == filter.SessionId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(x => x.Type == filter.Type);
            }

            return query;
        }
    }
}
