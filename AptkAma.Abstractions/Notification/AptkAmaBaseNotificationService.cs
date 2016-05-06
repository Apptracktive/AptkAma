using System.Collections.Generic;
using System.Threading;
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
            var cts = new CancellationTokenSource();
            return RegisterAsync(notificationTemplate, cts.Token);
        }

        /// <summary>
        /// Register to a specific Azure Push Notification
        /// </summary>
        /// <param name="notificationTemplate">Notification to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        public virtual Task<bool> RegisterAsync(IAptkAmaNotificationTemplate notificationTemplate, CancellationToken cancellationToken)
        {
            return RegisterAsync(new List<IAptkAmaNotificationTemplate> { notificationTemplate }, cancellationToken);
        }

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="notifications">Notifications to register for</param>
        public virtual Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications)
        {
            var cts = new CancellationTokenSource();
            return RegisterAsync(notifications, cts.Token);
        }

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="notifications">Notifications to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        public abstract Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications, CancellationToken cancellationToken);

        /// <summary>
        /// Unregister from all Azure Push Notifications
        /// </summary>
        public Task<bool> UnregisterAsync()
        {
            var cts = new CancellationTokenSource();
            return UnregisterAsync(cts.Token);
        }

        /// <summary>
        /// Unregister from all Azure Push Notifications
        /// </summary>
        /// <param name="cancellationToken">Token to cancel unregistration</param>
        public abstract Task<bool> UnregisterAsync(CancellationToken cancellationToken);
    }
}
