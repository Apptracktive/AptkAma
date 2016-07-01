using System.IO;
using System.Net.Http;
using Aptk.Plugins.AptkAma;
using Aptk.Plugins.AptkAma.Data;
using Aptk.Plugins.AptkAma.Identity;
using Aptk.Plugins.AptkAma.Notification;
using AptkAma.Sample.Core;
using AptkAma.Sample.Core.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace AptkAma.Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();

            InitAptkAmaPlugin(app);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        private void InitAptkAmaPlugin(UIApplication app)
        {
            var configuration = new AptkAmaPluginConfiguration(Constants.AmsUrl, Constants.ModelAssembly);

            // [Optional] Handle expired token to automaticaly ask for login if needed
            var identityHandler = new AptkAmaIdentityHandler(configuration);
            configuration.HttpHandlers = new HttpMessageHandler[] { identityHandler };

            // [Optional] Manage local caching
            configuration.CacheService = new AptkAmaCacheService();

            // [Optional] Handle notifications
            configuration.NotificationHandler = new AptkAmaNotificationHandler();

            // Init main plugin
            AptkAmaPluginLoader.Init(configuration, app);

            // [Optional] If AptkAmaIdentityHandler is used, give it an instance of the plugin after Init
            identityHandler.AptkAmaService = AptkAmaPluginLoader.Instance;

            // Init local store extension
            AptkAmaLocalStorePluginLoader.Init(new AptkAmaLocalStorePluginConfiguration(AptkAmaFileStorePluginLoader.Instance));
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            AptkAmaPluginLoader.Instance.Notification.RegisteredForRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            AptkAmaPluginLoader.Instance.Notification.FailedToRegisterForRemoteNotifications(error);
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            AptkAmaPluginLoader.Instance.Notification.ReceivedRemoteNotification(userInfo);
        }
    }
}


