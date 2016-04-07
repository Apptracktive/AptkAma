using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Service to manage Push Notifications
    /// </summary>
    public abstract class AptkAmaBaseNotificationService : IAptkAmaNotificationService
    {
        public readonly IMobileServiceClient Client;
        public readonly IAptkAmaPluginConfiguration Configuration;
        public TaskCompletionSource<bool> Tcs;
        public IEnumerable<IAptkAmaNotificationTemplate> PendingRegistrations;
        public IEnumerable<IAptkAmaNotificationTemplate> PendingUnregistrations;

        protected AptkAmaBaseNotificationService(IMobileServiceClient client, IAptkAmaPluginConfiguration configuration)
        {
            Client = client;
            Configuration = configuration;
        }

        /// <summary>
        /// Register to a specific Azure Push Notification
        /// </summary>
        /// <param name="notificationTemplate">Notification to register for</param>
        public virtual Task<bool> RegisterAsync(IAptkAmaNotificationTemplate notificationTemplate)
        {
            return RegisterAsync(new List<IAptkAmaNotificationTemplate> { notificationTemplate });
        }

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="notifications">Notifications to register for</param>
        public abstract Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications);

        /// <summary>
        /// Unregister from all Azure Push Notifications
        /// </summary>
        public virtual Task<bool> UnregisterAllAsync()
        {
            return UnregisterAsync(new List<IAptkAmaNotificationTemplate>());
        }

        /// <summary>
        /// Unregister from a specific Azure Push Notification
        /// </summary>
        public virtual Task<bool> UnregisterAsync(IAptkAmaNotificationTemplate notificationTemplate)
        {
            return UnregisterAsync(new List<IAptkAmaNotificationTemplate> { notificationTemplate });
        }

        /// <summary>
        /// Unregister from Azure Push Notifications
        /// </summary>
        public abstract Task<bool> UnregisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications);
    }
}
