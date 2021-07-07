namespace IdentityServer4.MongoDB.Database
{
    using global::MongoDB.Driver;

    /// <summary>
    /// an accessor to the IdentityServer operational database
    /// </summary>
    public interface IPersistedGrantDatabaseAccessor
    {
        /// <summary>
        /// the mongoDb database instance
        /// </summary>
        IMongoDatabase Database { get; }
    }
}
