
using Aptk.Plugins.AptkAma.Api;
using Aptk.Plugins.AptkAma.Data;
using Aptk.Plugins.AptkAma.Identity;
using Aptk.Plugins.AptkAma.Notification;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma
{
    /// <summary>
    /// AptkAma main plugin service
    /// </summary>
    public interface IAptkAmaService
    {
        /// <summary>
        /// Current plugin configuration
        /// </summary>
        IAptkAmaPluginConfiguration Configuration { get; }

        /// <summary>
        /// Current Mobile Service Client
        /// </summary>
        IMobileServiceClient Client { get; }

        /// <summary>
        /// Service to manage data
        /// </summary>
        IAptkAmaDataService Data { get; }

        /// <summary>
        /// Service to handle user Authentication
        /// </summary>
        IAptkAmaIdentityService Identity { get; }

        /// <summary>
        /// Service to send custom request
        /// </summary>
        IAptkAmaApiService Api { get; }

        /// <summary>
        /// Service to manage Push Notifications
        /// </summary>
        IAptkAmaNotificationService Notification { get; }
    }
}
