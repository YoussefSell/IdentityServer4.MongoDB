namespace IdentityServer4.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Options;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Helper to cleanup stale persisted grants and device codes.
    /// </summary>
    public class TokenCleanupService
    {
        private readonly OperationalStoreOptions _options;
        private readonly IMongoCollection<PersistedGrantEntity> _persistedGrantCollection;
        private readonly IMongoCollection<DeviceCodeEntity> _deviceFlowCodesCollection;
        private readonly IOperationalStoreNotification _operationalStoreNotification;
        private readonly ILogger<TokenCleanupService> _logger;

        /// <summary>
        /// Constructor for TokenCleanupService.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="persistedGrantCollection">the persistedGrant collection.</param>
        /// <param name="deviceFlowCodesCollection">the DeviceFlowCodes collection.</param>
        /// <param name="operationalStoreNotification">the operationalStoreNotification instance</param>
        /// <param name="logger">the logger instance</param>
        public TokenCleanupService(
            OperationalStoreOptions options,
            IMongoCollection<PersistedGrantEntity> persistedGrantCollection,
            IMongoCollection<DeviceCodeEntity> deviceFlowCodesCollection,
            ILogger<TokenCleanupService> logger,
            IOperationalStoreNotification operationalStoreNotification = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.TokenCleanupBatchSize < 1) throw new ArgumentException("Token cleanup batch size interval must be at least 1");

            _persistedGrantCollection = persistedGrantCollection ?? throw new ArgumentNullException(nameof(persistedGrantCollection));
            _deviceFlowCodesCollection = deviceFlowCodesCollection ?? throw new ArgumentNullException(nameof(deviceFlowCodesCollection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _operationalStoreNotification = operationalStoreNotification;
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                await RemoveDeviceCodesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveGrantsAsync()
        {
            IEnumerable<PersistedGrant> expiredGrants = null;

            if (!(_operationalStoreNotification is null))
            {
                expiredGrants = await _persistedGrantCollection.AsQueryable()
                    .Where(x => x.Expiration < DateTime.UtcNow)
                    .ToListAsync();
            }

            _logger.LogInformation("performing grants cleanup...");
            await _persistedGrantCollection.DeleteManyAsync(grant => grant.Expiration < DateTime.UtcNow);

            if (_operationalStoreNotification != null)
                await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants);
        }

        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveDeviceCodesAsync()
        {
            IEnumerable<DeviceCodeEntity> expiredCodes = null;

            if (!(_operationalStoreNotification is null))
            {
                expiredCodes = await _deviceFlowCodesCollection.AsQueryable()
                    .Where(codes => codes.Expiration < DateTime.UtcNow)
                    .ToListAsync();
            }

            _logger.LogInformation("performing codes cleanup...");
            await _deviceFlowCodesCollection.DeleteManyAsync(codes => codes.Expiration < DateTime.UtcNow);

            if (_operationalStoreNotification != null)
                await _operationalStoreNotification.DeviceCodesRemovedAsync(expiredCodes);
        }
    }
}
