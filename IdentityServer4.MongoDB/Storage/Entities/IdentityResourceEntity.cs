namespace IdentityServer4.MongoDB.Entities
{
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Utilities;

    /// <summary>
    /// this class defines <see cref="IdentityResource"/> entity
    /// </summary>
    public class IdentityResourceEntity : IdentityResource
    {
        /// <summary>
        /// create an instance of <see cref="IdentityResourceEntity"/>
        /// </summary>
        public IdentityResourceEntity()
        {
            Id = MongoIdGenerator.GenerateId("IDRS");
        }

        /// <summary>
        /// the unique identifier of the entity
        /// </summary>
        public string Id { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Id;
    }
}
