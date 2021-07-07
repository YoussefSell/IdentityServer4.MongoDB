namespace IdentityServer4.MongoDB.Services
{
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using IdentityServer4.MongoDB.Entities;
    using IdentityServer4.Services;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of ICorsPolicyService that consults the client configuration in the database for allowed CORS origins.
    /// </summary>
    /// <seealso cref="ICorsPolicyService" />
    public partial class CorsPolicyService
    {
        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>true if allowed, false if not</returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            origin = origin.ToLowerInvariant();

            var isAllowed = await _context.AsQueryable()
                .AnyAsync(client => client.AllowedCorsOrigins.Contains(origin));

            _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return isAllowed;
        }
    }

    /// <summary>
    /// partial part for <see cref="CorsPolicyService"/>
    /// </summary>
    public partial class CorsPolicyService : ICorsPolicyService
    {
        private readonly IMongoCollection<ClientEntity> _context;
        private readonly ILogger<CorsPolicyService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public CorsPolicyService(IMongoCollection<ClientEntity> context, ILogger<CorsPolicyService> logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}
