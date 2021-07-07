namespace IdentityServer4.MongoDB.Entities
{
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Utilities;

    /// <summary>
    /// this class defines <see cref="ApiScope"/> entity
    /// </summary>
    public class ApiScopeEntity : ApiScope
    {
        /// <summary>
        /// create an instance of <see cref="ApiScopeEntity"/>
        /// </summary>
        public ApiScopeEntity()
        {
            Id = MongoIdGenerator.GenerateId("ASCP");
        }

        /// <summary>
        /// the unique identifier of the entity
        /// </summary>
        public string Id { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Id;
    }
}
