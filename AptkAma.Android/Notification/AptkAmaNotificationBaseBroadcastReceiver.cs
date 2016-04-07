using Gcm;

namespace Aptk.Plugins.AptkAma.Notification
{
    public abstract class AptkAmaNotificationBaseBroadcastReceiver : GcmBroadcastReceiverBase<AptkAmaNotificationGcmService>
    {
        public static string[] SenderIds { get; protected set; }
    }
}