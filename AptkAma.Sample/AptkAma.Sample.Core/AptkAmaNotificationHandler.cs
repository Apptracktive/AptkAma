using Aptk.Plugins.AptkAma.Notification;

namespace AptkAma.Sample.Core
{
    public class AptkAmaNotificationHandler : AptkAmaBaseNotificationHandler
    {
        public static IAptkAmaNotificationTemplate TestNotificationTemplate = new AptkAmaNotificationTemplate("MyTemplate")
        {
            {"name", "$(name)"},
            {"alert", "$(message)"},
            {"sound", "default"}
        };

        public AptkAmaNotificationHandler()
        {
            GoogleSenderIds = Constants.GoogleSenderIds;
        }

        public override void OnNotificationReceived(IAptkAmaNotification notification)
        {
            if (notification.IsTypeOf(TestNotificationTemplate))
            {
            }
        }

        public override void OnUnregistrationSucceeded()
        {
            base.OnUnregistrationSucceeded();
        }

        public override void OnRegistrationSucceeded()
        {
            base.OnRegistrationSucceeded();
        }
    }
}
