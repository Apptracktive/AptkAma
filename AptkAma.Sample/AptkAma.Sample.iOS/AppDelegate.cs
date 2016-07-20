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

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
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


