using Android.App;
using Android.Content;
using Gcm;

[assembly: UsesPermission(Android.Manifest.Permission.ReceiveBootCompleted)]
[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace Aptk.Plugins.AptkAma.Notification
{
    [BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })] // Allow GCM on boot and when app is closed
    [IntentFilter(new[] { Constants.INTENT_FROM_GCM_MESSAGE },
        Categories = new[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
        Categories = new[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
        Categories = new[] { "@PACKAGE_NAME@" })]
    public class AptkAmaNotificationBroadcastReceiver : AptkAmaNotificationBaseBroadcastReceiver
    {
        public AptkAmaNotificationBroadcastReceiver()
        {
            SenderIds = AptkAmaPluginLoader.Instance.Configuration.NotificationHandler?.GoogleSenderIds;
        }
    }
}