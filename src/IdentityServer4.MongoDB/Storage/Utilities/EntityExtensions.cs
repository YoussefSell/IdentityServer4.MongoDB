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

        /// <summary>
        /// convert the given <see cref="Client"/> to the an instance of <see cref="ClientEntity"/>
        /// </summary>
        /// <param name="client">the <see cref="Client"/> to be converted</param>
        /// <returns>an instance of <see cref="ClientEntity"/></returns>
        public static ClientEntity ToEntity(this Client client)
        {
            return new ClientEntity
            {
                Claims = client.Claims,
                Enabled = client.Enabled,
                LogoUri = client.LogoUri,
                ClientId = client.ClientId,
                ClientUri = client.ClientUri,
                ClientName = client.ClientName,
                Properties = client.Properties,
                Description = client.Description,
                RequirePkce = client.RequirePkce,
                ProtocolType = client.ProtocolType,
                UserCodeType = client.UserCodeType,
                RedirectUris = client.RedirectUris,
                IncludeJwtId = client.IncludeJwtId,
                AllowedScopes = client.AllowedScopes,
                ClientSecrets = client.ClientSecrets,
                RequireConsent = client.RequireConsent,
                UserSsoLifetime = client.UserSsoLifetime,
                ConsentLifetime = client.ConsentLifetime,
                AccessTokenType = client.AccessTokenType,
                EnableLocalLogin = client.EnableLocalLogin,
                AllowedGrantTypes = client.AllowedGrantTypes,
                RefreshTokenUsage = client.RefreshTokenUsage,
                DeviceCodeLifetime = client.DeviceCodeLifetime,
                ClientClaimsPrefix = client.ClientClaimsPrefix,
                AllowPlainTextPkce = client.AllowPlainTextPkce,
                AllowOfflineAccess = client.AllowOfflineAccess,
                AllowedCorsOrigins = client.AllowedCorsOrigins,
                RequireClientSecret = client.RequireClientSecret,
                PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                AccessTokenLifetime = client.AccessTokenLifetime,
                RequireRequestObject = client.RequireRequestObject,
                AllowRememberConsent = client.AllowRememberConsent,
                BackChannelLogoutUri = client.BackChannelLogoutUri,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                PostLogoutRedirectUris = client.PostLogoutRedirectUris,
                AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                RefreshTokenExpiration = client.RefreshTokenExpiration,
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                IdentityProviderRestrictions = client.IdentityProviderRestrictions,
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                AllowedIdentityTokenSigningAlgorithms = client.AllowedIdentityTokenSigningAlgorithms,
            };
        }

    }
}
