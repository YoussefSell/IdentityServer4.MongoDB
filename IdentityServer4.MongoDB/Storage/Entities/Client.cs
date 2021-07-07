namespace IdentityServer4.MongoDB.Entities
{
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Utilities;

    /// <summary>
    /// this class defines <see cref="Client"/> entity
    /// </summary>
    public class ClientEntity : Client 
    {
        /// <summary>
        /// create an instance of <see cref="ClientEntity"/>
        /// </summary>
        public ClientEntity()
        {
            Id = MongoIdGenerator.GenerateId("CLNT");
        }

        /// <summary>
        /// the unique identifier of the entity
        /// </summary>
        public string Id { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Id;
    }
}
