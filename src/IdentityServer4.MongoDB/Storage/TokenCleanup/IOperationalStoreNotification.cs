namespace IdentityServer4.MongoDB
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.MongoDB.Entities;

    /// <summary>
    /// Interface to model notifications from the TokenCleanup feature.
    /// </summary>
    public interface IOperationalStoreNotification
    {
        /// <summary>
        /// Notification for persisted grants being removed.
        /// </summary>
        /// <param name="persistedGrants"></param>
        /// <returns></returns>
        Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants);

        /// <summary>
        /// Notification for device codes being removed.
        /// </summary>
        /// <param name="deviceCodes"></param>
        /// <returns></returns>
        Task DeviceCodesRemovedAsync(IEnumerable<DeviceCodeEntity> deviceCodes);
    }
}