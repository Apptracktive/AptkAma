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
        /// <param name="template">Notification template to register for</param>
        public virtual Task<bool> RegisterAsync(IAptkAmaNotificationTemplate template)
        {
            var cts = new CancellationTokenSource();
            return RegisterAsync(template, cts.Token);
        }

        /// <summary>
        /// Register to a specific Azure Push Notification
        /// </summary>
        /// <param name="template">Notification template to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        public virtual Task<bool> RegisterAsync(IAptkAmaNotificationTemplate template, CancellationToken cancellationToken)
        {
            return RegisterAsync(new List<IAptkAmaNotificationTemplate> { template }, cancellationToken);
        }

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="templates">Notification templates to register for</param>
        public virtual Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> templates)
        {
            var cts = new CancellationTokenSource();
            return RegisterAsync(templates, cts.Token);
        }

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="templates">Notification templates to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        public abstract Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> templates, CancellationToken cancellationToken);

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
