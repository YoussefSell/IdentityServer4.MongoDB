namespace IdentityServer4.MongoDB.Options
{
    using global::MongoDB.Driver;

    /// <summary>
    /// the base store configuration options
    /// </summary>
    public class BaseStoreOptions
    {
        /// <summary>
        /// the options to configure the database connection
        /// </summary>
        public DatabaseOptions DatabaseOptions { get; set; }

        /// <summary>
        /// get or set the registration scope of the services
        /// </summary>
        public RegistrationScope RegistrationScope { get; set; } = RegistrationScope.Scoped;

        /// <summary>
        /// use this function for a quick configuration of the database options.
        /// </summary>
        /// <param name="databaseName">the name of the database</param>
        /// <param name="connectionString">the connection string</param>
        public void Connect(string databaseName, string connectionString)
        {
            DatabaseOptions = new DatabaseOptions(databaseName, MongoClientSettings.FromConnectionString(connectionString));
        }
    }
}