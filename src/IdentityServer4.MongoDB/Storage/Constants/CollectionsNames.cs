namespace IdentityServer4.MongoDB.Constants
{
    /// <summary>
    /// holds the names of the default database collections
    /// </summary>
    internal static class CollectionsNames
    {
        /// <summary>
        /// the clients collection name
        /// </summary>
        public const string ClientsCollections = "Clients";

        /// <summary>
        /// the clients collection name
        /// </summary>
        public const string ApiResourcesCollections = "ApiResources";

        /// <summary>
        /// the IdentityResources collection name
        /// </summary>
        public const string IdentityResourcesCollections = "IdentityResources";

        /// <summary>
        /// the ApiScopes collection name
        /// </summary>
        public const string ApiScopesCollections = "ApiScopes";

        /// <summary>
        /// the PersistedGrants collection name
        /// </summary>
        public static string PersistedGrants = "PersistedGrants";

        /// <summary>
        /// the DeviceCodes collection name
        /// </summary>
        public static string DeviceCodes = "DeviceCodes";
    }
}
