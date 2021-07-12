namespace IdentityServer4.MongoDB.Entities
{
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Utilities;

    /// <summary>
    /// this class defines <see cref="IdentityResource"/> entity
    /// </summary>
    public class PersistedGrantEntity : PersistedGrant
    {
        /// <summary>
        /// create an instance of <see cref="PersistedGrantEntity"/>
        /// </summary>
        public PersistedGrantEntity()
        {
            Id = MongoIdGenerator.GenerateId("PRGT");
        }

        /// <summary>
        /// the unique identifier of the entity
        /// </summary>
        public string Id { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Id;

        /// <summary>
        /// map the given grant instance to a persisted grant instance
        /// </summary>
        /// <param name="grant">the grant to be mapped</param>
        /// <returns>instance of <see cref="PersistedGrantEntity"/></returns>
        public static PersistedGrantEntity MapFrom(PersistedGrant grant)
        {
            return new PersistedGrantEntity
            {
                Key = grant.Key,
                Type = grant.Type,
                Data = grant.Data,
                ClientId = grant.ClientId,
                SubjectId = grant.SubjectId,
                SessionId = grant.SessionId,
                Expiration = grant.Expiration,
                Description = grant.Description,
                ConsumedTime = grant.ConsumedTime,
                CreationTime = grant.CreationTime,
            };
        }
    }
}
