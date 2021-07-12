namespace IdentityServer4.MongoDB.Database
{
    using global::MongoDB.Driver;

    /// <summary>
    /// an accessor to the IdentityServer configuration database.
    /// </summary>
    public interface IConfigurationDatabaseAccessor
    {
        /// <summary>
        /// the mongoDb database instance
        /// </summary>
        IMongoDatabase Database { get; }
    }
}
