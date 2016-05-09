using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Service to manage Push Notifications
    /// </summary>
    public interface IAptkAmaNotificationService
    {
        /// <summary>
        /// Register to a specific Azure Push Notification
        /// </summary>
        /// <param name="template">Notification template to register for</param>
        Task<bool> RegisterAsync(IAptkAmaNotificationTemplate template);

        /// <summary>
        /// Register to a specific Azure Push Notification
        /// </summary>
        /// <param name="template">Notification template to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        Task<bool> RegisterAsync(IAptkAmaNotificationTemplate template, CancellationToken cancellationToken);

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="templates">Notification templates to register for</param>
        Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> templates);

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="templates">Notification templates to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> templates, CancellationToken cancellationToken);

        /// <summary>
        /// Unregister from all Azure Push Notifications
        /// </summary>
        Task<bool> UnregisterAsync();

        /// <summary>
        /// Unregister from all Azure Push Notifications
        /// </summary>
        /// <param name="cancellationToken">Token to cancel unregistration</param>
        Task<bool> UnregisterAsync(CancellationToken cancellationToken);
    }
}
