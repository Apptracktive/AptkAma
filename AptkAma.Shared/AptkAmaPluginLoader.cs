using System;
using Aptk.Plugins.AptkAma.Api;
using Aptk.Plugins.AptkAma.Data;
using Aptk.Plugins.AptkAma.Identity;
using Aptk.Plugins.AptkAma.Notification;
using Microsoft.WindowsAzure.MobileServices;

#if __IOS__
using UIKit;
#endif

#if __ANDROID__
using Android.App;
using Android.Content;
#endif

namespace Aptk.Plugins.AptkAma
{
    /// <summary>
    /// AptkAma plugin loader
    /// </summary>
    public static class AptkAmaPluginLoader
    {
        private static readonly Lazy<IAptkAmaService> LazyInstance = new Lazy<IAptkAmaService>(CreateAptkAmaService, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        private static readonly Lazy<IAptkAmaApiService> LazyApiInstance = new Lazy<IAptkAmaApiService>(CreateAptkAmaApiService, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        private static readonly Lazy<IAptkAmaDataService> LazyDataInstance = new Lazy<IAptkAmaDataService>(CreateAptkAmaDataService, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        private static readonly Lazy<IAptkAmaIdentityService> LazyIdentityInstance = new Lazy<IAptkAmaIdentityService>(CreateAptkAmaIdentityService, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        private static readonly Lazy<IAptkAmaPlatformIdentityService> LazyPlatformIdentityInstance = new Lazy<IAptkAmaPlatformIdentityService>(CreateAptkAmaPlatformIdentityService, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        private static readonly Lazy<IAptkAmaNotificationService> LazyNotificationInstance = new Lazy<IAptkAmaNotificationService>(CreateAptkAmaNotificationService, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static IAptkAmaPluginConfiguration _configuration;
        private static IMobileServiceClient _client;

        #region Init
#if __ANDROID__

        /// <summary>
        /// Initialize Android plugin
        /// </summary>
        public static void Init(IAptkAmaPluginConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _client = CreateMobileServiceClient();
            _context = context;
        }

        private static Context _context;

#elif __IOS__

        /// <summary>
        /// Initialize iOS plugin
        /// </summary>
        public static void Init(IAptkAmaPluginConfiguration configuration, UIApplication application)
        {
            _configuration = configuration;
            _client = CreateMobileServiceClient();
            _application = application;
        }

        private static UIApplication _application;

#else

        public static void Init(IAptkAmaPluginConfiguration configuration)
        {
            _configuration = configuration;
            _client = CreateMobileServiceClient();
        }

#endif

        private static IMobileServiceClient CreateMobileServiceClient()
        {
            var client = new MobileServiceClient(_configuration.AmsUrl, _configuration.AmsKey, _configuration.HttpHandlers);

            if (_configuration.SerializerSettings != null)
                client.SerializerSettings = _configuration.SerializerSettings;

            return client;
        }
        #endregion

        #region Instance
        /// <summary>
        /// Current plugin instance
        /// </summary>
        public static IAptkAmaService Instance
        {
            get
            {
                if (_client == null)
                {
                    throw new Exception("You must call Init method before using it.");
                }
                var instance = LazyInstance.Value;
                if (instance == null)
                {
                    throw new PlatformNotSupportedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return instance;
            }
        }

        private static IAptkAmaService CreateAptkAmaService()
        {
            return new AptkAmaService(_configuration, _client);
        }
        #endregion

        #region ApiInstance
        /// <summary>
        /// Current Api instance to use
        /// </summary>
        internal static IAptkAmaApiService ApiInstance
        {
            get
            {
                var apiInstance = LazyApiInstance.Value;
                if (apiInstance == null)
                {
                    throw new PlatformNotSupportedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return apiInstance;
            }
        }

        private static IAptkAmaApiService CreateAptkAmaApiService()
        {
            return new AptkAmaApiService(_configuration, _client);
        }
        #endregion

        #region DataInstance
        /// <summary>
        /// Current Data instance to use
        /// </summary>
        internal static IAptkAmaDataService DataInstance
        {
            get
            {
                var dataInstance = LazyDataInstance.Value;
                if (dataInstance == null)
                {
                    throw new PlatformNotSupportedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return dataInstance;
            }
        }

        private static IAptkAmaDataService CreateAptkAmaDataService()
        {
            return new AptkAmaDataService(_configuration, _client);
        }
        #endregion

        #region IdentityInstance
        /// <summary>
        /// Current Identity instance to use
        /// </summary>
        internal static IAptkAmaIdentityService IdentityInstance
        {
            get
            {
                var identityInstance = LazyIdentityInstance.Value;
                if (identityInstance == null)
                {
                    throw new PlatformNotSupportedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return identityInstance;
            }
        }

        private static IAptkAmaIdentityService CreateAptkAmaIdentityService()
        {
            return new AptkAmaIdentityService(_configuration, _client);
        }
        #endregion

        #region PlatformIdentityInstance
        /// <summary>
        /// Current Platform Identity instance to use
        /// </summary>
        internal static IAptkAmaPlatformIdentityService PlatformIdentityInstance
        {
            get
            {
                var platformIdentityInstance = LazyPlatformIdentityInstance.Value;
                if (platformIdentityInstance == null)
                {
                    throw new PlatformNotSupportedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return platformIdentityInstance;
            }
        }

        private static IAptkAmaPlatformIdentityService CreateAptkAmaPlatformIdentityService()
        {
#if PORTABLE
            return null;
#elif __ANDROID__
            return new AptkAmaPlatformIdentityService(_client, _context);
#elif __IOS__
            return new AptkAmaPlatformIdentityService(_client, _application);
#else
            return new AptkAmaPlatformIdentityService(_client);
#endif
        }
        #endregion

        #region NotificationInstance
        /// <summary>
        /// Current Notification instance to use
        /// </summary>
        internal static IAptkAmaNotificationService NotificationInstance
        {
            get
            {
                if (_configuration.NotificationHandler == null)
                    throw new ArgumentNullException($"You must provide a {nameof(_configuration.NotificationHandler)} to the plugin configuration before calling Notification methods");

                var notificationInstance = LazyNotificationInstance.Value;
                if (notificationInstance == null)
                {
                    throw new PlatformNotSupportedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return notificationInstance;
            }
        }

        private static IAptkAmaNotificationService CreateAptkAmaNotificationService()
        {
#if PORTABLE
            return null;
#elif __ANDROID__
            return new AptkAmaNotificationService(_client, _configuration, _context);
#else
            return new AptkAmaNotificationService(_client, _configuration);
#endif
        }
        #endregion

#if PORTABLE
#else
        #region PushInstance
        /// <summary>
        /// Get platform specific Push instance for custom implementation (Use Notification instead)
        /// </summary>
        /// <param name="pluginInstance">Main plugin instance</param>
        /// <returns>Push instance for for custom implementation</returns>
        public static Push Push(this IAptkAmaService pluginInstance)
        {
            return ((MobileServiceClient) _client).GetPush();
        }
        #endregion
#endif
    }
}
