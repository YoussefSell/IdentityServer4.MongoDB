namespace IdentityServer4.MongoDB.Entities
{
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Utilities;

    /// <summary>
    /// this class defines <see cref="ApiResource"/> entity
    /// </summary>
    public class ApiResourceEntity : ApiResource
    {
        /// <summary>
        /// create an instance of <see cref="ApiResourceEntity"/>
        /// </summary>
        public ApiResourceEntity()
        {
            Id = MongoIdGenerator.GenerateId("ARSR");
        }

        /// <summary>
        /// the unique identifier of the entity
        /// </summary>
        public string Id { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Id;
    }
}
