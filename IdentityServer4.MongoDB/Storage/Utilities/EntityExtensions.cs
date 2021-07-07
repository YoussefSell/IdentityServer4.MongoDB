namespace IdentityServer4.MongoDB.Entities
{
    using IdentityServer4.Models;

    /// <summary>
    /// extensions class for the entities
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// convert the given <see cref="PersistedGrant"/> to the an instance of <see cref="PersistedGrantEntity"/>
        /// </summary>
        /// <param name="grant">the <see cref="PersistedGrant"/> to be converted</param>
        /// <returns>an instance of <see cref="PersistedGrantEntity"/></returns>
        public static PersistedGrantEntity ToEntity(this PersistedGrant grant)
        {
            return new PersistedGrantEntity
            {
                Key = grant.Key,
                Data = grant.Data,
                Type = grant.Type,
                ClientId = grant.ClientId,
                SessionId = grant.SessionId,
                SubjectId = grant.SubjectId,
                Expiration = grant.Expiration,
                Description = grant.Description,
                ConsumedTime = grant.ConsumedTime,
                CreationTime = grant.CreationTime,
            };
        }

        /// <summary>
        /// convert the given <see cref="ApiResource"/> to the an instance of <see cref="ApiResourceEntity"/>
        /// </summary>
        /// <param name="grant">the <see cref="ApiResource"/> to be converted</param>
        /// <returns>an instance of <see cref="ApiResourceEntity"/></returns>
        public static ApiResourceEntity ToEntity(this ApiResource grant)
        {
            return new ApiResourceEntity
            {
                Name = grant.Name,
                Scopes = grant.Scopes,
                Enabled = grant.Enabled,
                UserClaims = grant.UserClaims,
                Properties = grant.Properties,
                ApiSecrets = grant.ApiSecrets,
                Description = grant.Description,
                DisplayName = grant.DisplayName,
                ShowInDiscoveryDocument = grant.ShowInDiscoveryDocument,
                AllowedAccessTokenSigningAlgorithms = grant.AllowedAccessTokenSigningAlgorithms,
            };
        }

        /// <summary>
        /// convert the given <see cref="ApiScope"/> to the an instance of <see cref="ApiScopeEntity"/>
        /// </summary>
        /// <param name="grant">the <see cref="ApiScope"/> to be converted</param>
        /// <returns>an instance of <see cref="ApiScopeEntity"/></returns>
        public static ApiScopeEntity ToEntity(this ApiScope grant)
        {
            return new ApiScopeEntity
            {
                Name = grant.Name,
                Enabled = grant.Enabled,
                Required = grant.Required,
                Emphasize = grant.Emphasize,
                UserClaims = grant.UserClaims,
                Properties = grant.Properties,
                Description = grant.Description,
                DisplayName = grant.DisplayName,
                ShowInDiscoveryDocument = grant.ShowInDiscoveryDocument,
            };
        }

        /// <summary>
        /// convert the given <see cref="IdentityResource"/> to the an instance of <see cref="IdentityResourceEntity"/>
        /// </summary>
        /// <param name="grant">the <see cref="IdentityResource"/> to be converted</param>
        /// <returns>an instance of <see cref="IdentityResourceEntity"/></returns>
        public static IdentityResourceEntity ToEntity(this IdentityResource grant)
        {
            return new IdentityResourceEntity
            {
                Name = grant.Name,
                Enabled = grant.Enabled,
                Required = grant.Required,
                Emphasize = grant.Emphasize,
                UserClaims = grant.UserClaims,
                Properties = grant.Properties,
                Description = grant.Description,
                DisplayName = grant.DisplayName,
                ShowInDiscoveryDocument = grant.ShowInDiscoveryDocument,
            };
        }
    }
}
