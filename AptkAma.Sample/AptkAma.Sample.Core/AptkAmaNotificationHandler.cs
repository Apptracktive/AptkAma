using System.Diagnostics;
using Acr.UserDialogs;
using Aptk.Plugins.AptkAma.Notification;
using Newtonsoft.Json;

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
            Debug.WriteLine($"OnNotificationReceived fired for: {JsonConvert.SerializeObject(notification)}");
            if (notification.IsTypeOf(TestNotificationTemplate))
            {
                UserDialogs.Instance.Alert(notification["alert"].ToString());
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
