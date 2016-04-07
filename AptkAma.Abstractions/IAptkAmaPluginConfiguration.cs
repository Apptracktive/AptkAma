using System.Net.Http;
using System.Reflection;
using Aptk.Plugins.AptkAma.Identity;
using Aptk.Plugins.AptkAma.Notification;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma
{
    /// <summary>
    /// AptkAma plugin configuration
    /// </summary>
    public interface IAptkAmaPluginConfiguration
    {
        /// <summary>
        /// Azure Mobile Apps URL
        /// </summary>
        string AmsUrl { get; }

        /// <summary>
        /// Azure Mobile Apps Key
        /// </summary>
        string AmsKey { get; }

        /// <summary>
        /// Assembly hosting model classes (usually the same)
        /// </summary>
        Assembly ModelAssembly { get; }

        /// <summary>
        /// [Optional] Credential cache service to manage credentials storing on device
        /// </summary>
        IAptkAmaCacheService CacheService { get; set; }

        /// <summary>
        /// [Optional] Custom Http message handlers
        /// </summary>
        HttpMessageHandler[] HttpHandlers { get; set; }

        /// <summary>
        /// [Optional] Json serializer settings
        /// </summary>
        MobileServiceJsonSerializerSettings SerializerSettings { get; set; }

        /// <summary>
        /// [Optional] Handler to manage notifications
        /// </summary>
        IAptkAmaNotificationHandler NotificationHandler { get; set; }
    }
}
