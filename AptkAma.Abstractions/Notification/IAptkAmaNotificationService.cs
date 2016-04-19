using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="notificationTemplate">Notification to register for</param>
        Task<bool> RegisterAsync(IAptkAmaNotificationTemplate notificationTemplate);

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="notifications">Notifications to register for</param>
        Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications);

        /// <summary>
        /// Unregister from all Azure Push Notifications
        /// </summary>
        Task<bool> UnregisterAsync();
    }
}
