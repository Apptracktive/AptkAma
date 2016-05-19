using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
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
        /// <param name="templates">Notification templates to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        public override Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> templates, CancellationToken cancellationToken)
        {
            if (Tcs == null || Tcs.Task.IsCanceled || Tcs.Task.IsCompleted || Tcs.Task.IsFaulted)
            {
                Tcs = new TaskCompletionSource<bool>();
                cancellationToken.Register(() => Tcs.TrySetCanceled(), false);

                PendingRegistrations = templates;
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
        /// <param name="cancellationToken">Token to cancel unregistration</param>
        public override Task<bool> UnregisterAsync(CancellationToken cancellationToken)
        {
            if (Tcs == null || Tcs.Task.IsCanceled || Tcs.Task.IsCompleted || Tcs.Task.IsFaulted)
            {
                Tcs = new TaskCompletionSource<bool>();
                cancellationToken.Register(() => Tcs.TrySetCanceled(), false);

                this.Unregister();
            }
            return Tcs.Task;
        }
    }
}
