using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using UIKit;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Service to manage Push Notifications
    /// </summary>
    public class AptkAmaNotificationService : AptkAmaBaseNotificationService
    {
        internal NSData DeviceToken;

        internal AptkAmaNotificationService(IMobileServiceClient client, IAptkAmaPluginConfiguration configuration) : base(client, configuration)
        {
        }

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="notifications">Notifications to register for</param>
        public override Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications)
        {
            if (Tcs == null)
            {
                Tcs = new TaskCompletionSource<bool>();
                PendingRegistrations = notifications;
                if (DeviceToken == null)
                {
                    UIApplication.SharedApplication.InvokeOnMainThread(() => {

                        var version8 = new Version(8, 0);
                        if (new Version(UIDevice.CurrentDevice.SystemVersion) < version8)
                        {
                            var notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                            UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
                        }
                        else {
                            var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());
                            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                        }
                        UIApplication.SharedApplication.RegisterForRemoteNotifications();
                    });
                }
                else {
                    this.RegisteredForRemoteNotifications(DeviceToken);
                }
            }
            return Tcs.Task;
        }

        /// <summary>
        /// Unregister from Azure Push Notifications
        /// </summary>
        public override Task<bool> UnregisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications)
        {
            if (Tcs == null)
            {
                Tcs = new TaskCompletionSource<bool>();
                PendingUnregistrations = notifications;
                this.Unregister(DeviceToken);
            }
            return Tcs.Task;
        }
    }
}
