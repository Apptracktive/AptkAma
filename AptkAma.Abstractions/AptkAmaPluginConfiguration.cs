using System.Net.Http;
using System.Reflection;
using Aptk.Plugins.AptkAma.Notification;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma
{
    /// <summary>
    /// AptkAma Plugin configuration
    /// </summary>
    public class AptkAmaPluginConfiguration : IAptkAmaPluginConfiguration
    {
        /// <summary>
        /// AptkAma Plugin configuration constructor
        /// </summary>
        /// <param name="amsUrl">Azure Mobile Apps URL</param>
        /// <param name="modelAssembly">Model classes hosting Assembly (usually the same)</param>
        public AptkAmaPluginConfiguration(string amsUrl, Assembly modelAssembly)
        {
            AmsUrl = amsUrl;
            ModelAssembly = modelAssembly;
        }

        /// <summary>
        /// AptkAma Plugin configuration constructor
        /// </summary>
        /// <param name="amsUrl">Azure Mobile Apps URL</param>
        /// <param name="amsKey">Azure Mobile Apps Key</param>
        /// <param name="modelAssembly">Model classes hosting Assembly (usually the same)</param>
        public AptkAmaPluginConfiguration(string amsUrl, string amsKey, Assembly modelAssembly)
        {
            AmsUrl = amsUrl;
            AmsKey = amsKey;
            ModelAssembly = modelAssembly;
        }

        /// <summary>
        /// Azure Mobile Apps URL
        /// </summary>
        public string AmsUrl { get; }

        /// <summary>
        /// Azure Mobile Apps Key
        /// </summary>
        public string AmsKey { get; }

        /// <summary>
        /// Assembly hosting model classes (usually the same)
        /// </summary>
        public Assembly ModelAssembly { get; }

        /// <summary>
        /// [Optional] Cache service to manage cache storing on device
        /// </summary>
        public IAptkAmaCacheService CacheService { get; set; }

        /// <summary>
        /// [Optional] Custom Http message handlers
        /// </summary>
        public HttpMessageHandler[] HttpHandlers { get; set; }

        /// <summary>
        /// [Optional] Json serializer settings
        /// </summary>
        public MobileServiceJsonSerializerSettings SerializerSettings { get; set; }

        /// <summary>
        /// [Optional] Handler to manage notifications
        /// </summary>
        public IAptkAmaNotificationHandler NotificationHandler { get; set; }
    }
}
