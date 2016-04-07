using Aptk.Plugins.AptkAma.Api;
using Aptk.Plugins.AptkAma.Data;
using Aptk.Plugins.AptkAma.Identity;
using Aptk.Plugins.AptkAma.Notification;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma
{
    /// <summary>
    /// AptkAma main plugin
    /// </summary>
    public class AptkAmaService : IAptkAmaService
    {
        internal AptkAmaService(IAptkAmaPluginConfiguration configuration, IMobileServiceClient client)
        {
            Configuration = configuration;
            Client = client;
        }

        /// <summary>
        /// AptkAma plugin configuration
        /// </summary>
        public IAptkAmaPluginConfiguration Configuration { get; }

        /// <summary>
        /// Azure Mobile Apps client
        /// </summary>
        public IMobileServiceClient Client { get; }

        /// <summary>
        /// Service to manage data
        /// </summary>
        public IAptkAmaDataService Data => AptkAmaPluginLoader.DataInstance;

        /// <summary>
        /// Service to manage identity
        /// </summary>
        public IAptkAmaIdentityService Identity => AptkAmaPluginLoader.IdentityInstance;

        /// <summary>
        /// Service to send custom request
        /// </summary>
        public IAptkAmaApiService Api => AptkAmaPluginLoader.ApiInstance;

        /// <summary>
        /// Service to manage Push Notifications
        /// </summary>
        public IAptkAmaNotificationService Notification => AptkAmaPluginLoader.NotificationInstance;
    }
}