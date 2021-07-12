namespace IdentityServer4.MongoDB.Stores
{
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.Stores;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// the client stores
    /// </summary>
    public partial class ClientStore
    {
        /// <inheritdoc/>
        public async Task<Client> FindClientByIdAsync(string clientId)
            => await _collection.AsQueryable().SingleOrDefaultAsync(client => client.ClientId == clientId);
    }

    /// <summary>
    /// partial part for <see cref="ClientStore"/>
    /// </summary>
    public partial class ClientStore : IClientStore
    {
        private readonly IMongoCollection<ClientEntity> _collection;

        /// <summary>
        /// create an instance of <see cref="ClientStore"/>
        /// </summary>
        /// <param name="collection">the client collection</param>
        public ClientStore(IMongoCollection<ClientEntity> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }
    }
}
