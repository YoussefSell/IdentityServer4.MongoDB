namespace IdentityServer4.MongoDB.Options
{
    using global::MongoDB.Driver;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class to control a table's name and schema.
    /// </summary>
    public class CollectionConfiguration<TDocument>
    {
        /// <summary>
        /// create an instance of <see cref="CollectionConfiguration{TDocument}"/>
        /// </summary>
        /// <param name="collectionName">the name of the collection</param>
        public CollectionConfiguration(string collectionName)
            : this(collectionName, null, Array.Empty<CreateIndexModel<TDocument>>()) { }

        /// <summary>
        /// create an instance of <see cref="CollectionConfiguration{TDocument}"/>
        /// </summary>
        /// <param name="collectionName">the name of the collection</param>
        /// <param name="collectionSettings">the name of the collection</param>
        public CollectionConfiguration(string collectionName, MongoCollectionSettings collectionSettings)
            : this(collectionName, collectionSettings, Array.Empty<CreateIndexModel<TDocument>>()) { }

        /// <summary>
        /// create an instance of <see cref="CollectionConfiguration{TDocument}"/>
        /// </summary>
        /// <param name="collectionName">the name of the collection</param>
        /// <param name="indexes">the list of indexes to configure on the collection</param>
        public CollectionConfiguration(string collectionName, params CreateIndexModel<TDocument>[] indexes)
            : this(collectionName, null, indexes) { }

        /// <summary>
        /// create an instance of <see cref="CollectionConfiguration{TDocument}"/>
        /// </summary>
        /// <param name="collectionName">the name of the collection</param>
        /// <param name="collectionSettings">the name of the collection</param>
        /// <param name="indexes">the list of indexes to configure on the collection</param>
        public CollectionConfiguration(string collectionName, MongoCollectionSettings collectionSettings, params CreateIndexModel<TDocument>[] indexes)
        {
            Name = collectionName;
            Settings = collectionSettings;
            Indexes = new List<CreateIndexModel<TDocument>>(indexes);
        }

        /// <summary>
        /// the name of the collection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the collection settings
        /// </summary>
        public MongoCollectionSettings Settings { get; set; }

        /// <summary>
        /// the list of indexes to create on the collection
        /// </summary>
        public ICollection<CreateIndexModel<TDocument>> Indexes { get; set; }
    }
}
