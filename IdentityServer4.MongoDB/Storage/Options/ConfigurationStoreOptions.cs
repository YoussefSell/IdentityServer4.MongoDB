namespace IdentityServer4.MongoDB.Options
{
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.MongoDB.Constants;

    /// <summary>
    /// Options for configuring the configuration context.
    /// </summary>
    public class ConfigurationStoreOptions : BaseStoreOptions
    {
        /// <summary>
        /// Gets or sets the client collection configuration.
        /// </summary>
        public CollectionConfiguration<ClientEntity> Client { get; set; } = new CollectionConfiguration<ClientEntity>(CollectionsNames.ClientsCollections);

        /// <summary>
        /// Gets or sets the identity resource collection configuration.
        /// </summary>
        public CollectionConfiguration<IdentityResourceEntity> IdentityResource { get; set; } = new CollectionConfiguration<IdentityResourceEntity>(CollectionsNames.IdentityResourcesCollections);

        /// <summary>
        /// Gets or sets the API resource collection configuration.
        /// </summary>
        public CollectionConfiguration<ApiResourceEntity> ApiResource { get; set; } = new CollectionConfiguration<ApiResourceEntity>(CollectionsNames.ApiResourcesCollections);

        /// <summary>
        /// Gets or sets the scope collection configuration.
        /// </summary>
        public CollectionConfiguration<ApiScopeEntity> ApiScope { get; set; } = new CollectionConfiguration<ApiScopeEntity>(CollectionsNames.ApiScopesCollections);
    }
}
