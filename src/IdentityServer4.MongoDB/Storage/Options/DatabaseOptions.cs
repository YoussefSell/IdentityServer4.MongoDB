namespace IdentityServer4.MongoDB.Options
{
    using global::MongoDB.Driver;
    using System;

    /// <summary>
    /// options for configuring the mongoDB database
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// create an instance of <see cref="DatabaseOptions"/>.
        /// </summary>
        /// <param name="databaseName">the name of the database.</param>
        /// <param name="connectingString">the database connection string.</param>
        public DatabaseOptions(string databaseName, string connectingString)
            : this(databaseName, MongoClientSettings.FromConnectionString(connectingString), null) { }

        /// <summary>
        /// create an instance of <see cref="DatabaseOptions"/>.
        /// </summary>
        /// <param name="databaseName">the name of the database.</param>
        /// <param name="mongoClientSettings">the MongoDb Client settings.</param>
        public DatabaseOptions(string databaseName, MongoClientSettings mongoClientSettings)
            : this(databaseName, mongoClientSettings, null) { }

        /// <summary>
        /// create an instance of <see cref="DatabaseOptions"/>.
        /// </summary>
        /// <param name="databaseName">the name of the database.</param>
        /// <param name="mongoClientSettings">the mongo Client settings.</param>
        /// <param name="mongoDatabaseSettings">the mongo database settings.</param>
        public DatabaseOptions(string databaseName, MongoClientSettings mongoClientSettings, MongoDatabaseSettings mongoDatabaseSettings)
        {
            MongoDatabaseSettings = mongoDatabaseSettings;
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            MongoClientSettings = mongoClientSettings ?? throw new ArgumentNullException(nameof(mongoClientSettings));
        }

        /// <summary>
        /// The settings for a MongoDB client.
        /// </summary>
        public MongoClientSettings MongoClientSettings { get; set; }

        /// <summary>
        /// The settings used to access a database.
        /// </summary>
        public MongoDatabaseSettings MongoDatabaseSettings { get; set; }

        /// <summary>
        /// name of the database
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
