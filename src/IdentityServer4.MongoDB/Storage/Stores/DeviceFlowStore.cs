namespace  IdentityServer4.MongoDB.Stores
{
    using IdentityModel;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using IdentityServer4.Stores.Serialization;
    using Microsoft.Extensions.Logging;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using System;
    using System.Threading.Tasks;
    using IdentityServer4.MongoDB.Entities;

    /// <summary>
    /// Implementation of IDeviceFlowStore thats uses EF.
    /// </summary>
    /// <seealso cref="IDeviceFlowStore" />
    public partial class DeviceFlowStore
    {
        /// <inheritdoc/>
        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            var deviceFlowCodes = await _collection.AsQueryable().SingleOrDefaultAsync(code => code.DeviceCode == deviceCode);
            if (deviceFlowCodes is null)
            {
                Logger.LogDebug("{deviceCode} was not found in database", deviceCode);
                return default;
            }

            Logger.LogDebug("{deviceCode} found in database", deviceCode);
            return ToModel(deviceFlowCodes.Data);
        }

        /// <inheritdoc/>
        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            var deviceCode = await _collection.AsQueryable().SingleOrDefaultAsync(code => code.UserCode == userCode);
            if (deviceCode is null)
            {
                Logger.LogDebug("{userCode} was not found in database", userCode);
                return default;
            }

            Logger.LogDebug("{userCode} found in database", userCode);
            return ToModel(deviceCode.Data);
        }

        /// <inheritdoc/>
        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            var deleteResult = await _collection.DeleteOneAsync(code => code.DeviceCode == deviceCode);
        }

        /// <inheritdoc/>
        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            var entity = ToEntity(data, deviceCode, userCode);
            if (entity is null)
                return;

            await _collection.InsertOneAsync(entity);
        }

        /// <inheritdoc/>
        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            var existing = await _collection.AsQueryable()
                .SingleOrDefaultAsync(deviceCode => deviceCode.UserCode == userCode);

            if (existing == null)
            {
                Logger.LogError("{userCode} not found in database", userCode);
                throw new InvalidOperationException("Could not update device code");
            }

            var entity = ToEntity(data, existing.DeviceCode, userCode);
            if (entity is null)
            {
                Logger.LogDebug("{userCode} failed to update the user device code", userCode);
                return;
            }

            Logger.LogDebug("{userCode} found in database", userCode);

            var updateResult = await _collection.UpdateOneAsync(deviceCode => deviceCode.UserCode == userCode,
                update: Builders<DeviceCodeEntity>.Update
                    .Set(deviceCode => deviceCode.Data, entity.Data)
                    .Set(deviceCode => deviceCode.SubjectId, data.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value));
        }
    }

    /// <summary>
    /// partial part for <see cref="DeviceFlowStore"/>
    /// </summary>
    public partial class DeviceFlowStore : IDeviceFlowStore
    {
        /// <summary>
        /// The DeviceCode mongo collection.
        /// </summary>
        protected readonly IMongoCollection<DeviceCodeEntity> _collection;

        /// <summary>
        ///  The serializer.
        /// </summary>
        protected readonly IPersistentGrantSerializer Serializer;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceFlowStore"/> class.
        /// </summary>
        /// <param name="collection">The DeviceFlowCodes collection.</param>
        /// <param name="serializer">The serializer</param>
        /// <param name="logger">The logger.</param>
        public DeviceFlowStore(
            IMongoCollection<DeviceCodeEntity> collection,
            IPersistentGrantSerializer serializer,
            ILogger<DeviceFlowStore> logger)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            Serializer = serializer;
            Logger = logger;
        }

        /// <summary>
        /// Converts a model to an entity.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="deviceCode"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        protected DeviceCodeEntity ToEntity(DeviceCode model, string deviceCode, string userCode)
        {
            if (model is null || deviceCode is null || userCode is null)
                return null;

            return new DeviceCodeEntity
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                ClientId = model.ClientId,
                SubjectId = model.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value,
                CreationTime = model.CreationTime,
                Expiration = model.CreationTime.AddSeconds(model.Lifetime),
                Data = Serializer.Serialize(model)
            };
        }

        /// <summary>
        /// Converts a serialized DeviceCode to a model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected DeviceCode ToModel(string entity)
        {
            return entity is null ? null
                : Serializer.Deserialize<DeviceCode>(entity);
        }
    }
}
