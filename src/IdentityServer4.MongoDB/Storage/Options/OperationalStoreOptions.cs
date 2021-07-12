namespace IdentityServer4.MongoDB.Options
{
    using global::MongoDB.Driver;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Constants;

    /// <summary>
    /// Options for configuring the operational context.
    /// </summary>
    public class OperationalStoreOptions : BaseStoreOptions
    {
        /// <summary>
        /// Gets or sets the persisted grants collection configuration.
        /// </summary>
        public CollectionConfiguration<PersistedGrantEntity> PersistedGrant { get; set; } = new CollectionConfiguration<PersistedGrantEntity>(CollectionsNames.PersistedGrants);

        /// <summary>
        /// Gets or sets the device flow codes collection configuration.
        /// </summary>
        public CollectionConfiguration<DeviceCodeEntity> DeviceFlowCodes { get; set; }
            = new CollectionConfiguration<DeviceCodeEntity>(CollectionsNames.DeviceCodes, new[]
                { new CreateIndexModel<DeviceCodeEntity>(keys: "{deviceCode : 1}", options: new CreateIndexOptions{ Unique = true }) });

        /// <summary>
        /// Gets or sets a value indicating whether stale entries will be automatically cleaned up from the database.
        /// This is implemented by periodically connecting to the database (according to the TokenCleanupInterval) from the hosting application.
        /// Defaults to false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable token cleanup]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTokenCleanup { get; set; } = false;

        /// <summary>
        /// Gets or sets the token cleanup interval (in seconds). The default is 3600 (1 hour).
        /// </summary>
        /// <value>
        /// The token cleanup interval.
        /// </value>
        public int TokenCleanupInterval { get; set; } = 3600;

        /// <summary>
        /// Gets or sets the number of records to remove at a time. Defaults to 100.
        /// </summary>
        /// <value>
        /// The size of the token cleanup batch.
        /// </value>
        public int TokenCleanupBatchSize { get; set; } = 100;
    }
}
