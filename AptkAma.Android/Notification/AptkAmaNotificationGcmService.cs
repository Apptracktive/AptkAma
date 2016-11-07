using System.Diagnostics;
using Android.App;
using Android.Content;
using Gcm.Client;

namespace Aptk.Plugins.AptkAma.Notification
{
    [Service]
    public class AptkAmaNotificationGcmService : GcmServiceBase
    {
        public AptkAmaNotificationGcmService() : base(AptkAmaNotificationBaseBroadcastReceiver.SenderIds)
        {
        }

        protected override void OnRegistered(Context context, string registrationId)
        {
            AptkAmaPluginLoader.Instance.Notification.OnRegistered(context, registrationId);
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            AptkAmaPluginLoader.Instance.Notification.OnMessage(context, intent);
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            AptkAmaPluginLoader.Instance.Notification.OnUnRegistered(context, registrationId);
        }

        protected override void OnError(Context context, string errorId)
        {
            AptkAmaPluginLoader.Instance.Notification.OnError(context, errorId);
        }
    }
}