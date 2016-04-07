using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aptk.Plugins.AptkAma.Extensions;
using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using UIKit;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Extension methods to manage platform specific notification registration
    /// </summary>
    public static class AptkAmaNotificationServiceExtension
    {
        /// <summary>
        /// Call this method from the RegisteredForRemoteNotifications override into the AppDelegate to register for notifications
        /// </summary>
        /// <param name="notificationService">This notification service</param>
        /// <param name="deviceToken">The token of the device provided by the platform</param>
        /// <exception cref="ArgumentNullException">deviceToken should not be null</exception>
        public static void RegisteredForRemoteNotifications(this IAptkAmaNotificationService notificationService, NSData deviceToken)
        {
            if (deviceToken == null)
                throw new ArgumentNullException(nameof(deviceToken));

            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if(platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");

            if (platformNotificationService.Tcs == null)
                return;
            
            try
            {
                platformNotificationService.DeviceToken = deviceToken;
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();

                foreach (var pendingRegistration in platformNotificationService.PendingRegistrations)
                {
                    var expiryDate = DateTime.Now.Add(pendingRegistration.ExpiresAfter).ToString(pendingRegistration.CultureInfo);
                    var template = JsonConvert.SerializeObject(new Dictionary<string, object> { {"aps", pendingRegistration} });

                    UIApplication.SharedApplication.InvokeOnMainThread(async () =>
                    {
                        await push.RegisterTemplateAsync(platformNotificationService.DeviceToken, template, expiryDate, pendingRegistration.Name, pendingRegistration.Tags);
                            
                        Debug.WriteLine($"Notification {pendingRegistration.Name} registered with template {template} and tags {pendingRegistration.Tags}");

                        platformNotificationService.Configuration.NotificationHandler?.OnRegistrationSucceeded(pendingRegistration.Name);
                    });
                }

                UIApplication.SharedApplication.InvokeOnMainThread(() => platformNotificationService.Configuration.CacheService?.SaveRegistrationId(deviceToken.ToString()));
                platformNotificationService.Tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                platformNotificationService.Tcs.SetException(ex);
            }
            finally
            {
                platformNotificationService.PendingRegistrations = null;
                platformNotificationService.Tcs = null;
            }
        }

        /// <summary>
        /// Call this method from the FailedToRegisterForRemoteNotifications override into the AppDelegate to register for notifications
        /// </summary>
        /// <param name="notificationService">This notification service</param>
        /// <param name="error">Error returned</param>
        /// <exception cref="ArgumentNullException">error should not be null</exception>
        public static void FailedToRegisterForRemoteNotifications(this IAptkAmaNotificationService notificationService, NSError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if (platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");

            if (platformNotificationService.Tcs == null)
                return;

            var sb = new StringBuilder();
            try
            {
                sb.AppendFormat("Error Code:  {0}\r\n", error.Code);
                sb.AppendFormat("Description: {0}\r\n", error.LocalizedDescription);
                var userInfo = error.UserInfo;
                for (int i = 0; i < userInfo.Keys.Length; i++)
                {
                    sb.AppendFormat("[{0}]: {1}\r\n", userInfo.Keys[i], userInfo.Values[i]);
                }

                platformNotificationService.Tcs.SetException(new Exception(sb.ToString()));
            }
            catch (Exception ex)
            {
                platformNotificationService.Tcs.SetException(ex);
            }
            finally
            {
                platformNotificationService.PendingRegistrations = null;
                platformNotificationService.Tcs = null;
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    platformNotificationService.Configuration.CacheService?.ClearRegistrationId();
                    platformNotificationService.Configuration.NotificationHandler?.OnRegistrationFailed(sb.ToString());
                });
            }
        }

        /// <summary>
        /// Call this method from the ReceivedRemoteNotification override into the AppDelegate to register for notifications
        /// </summary>
        /// <param name="notificationService">This notification service</param>
        /// <param name="userInfo">User info received</param>
        /// <exception cref="ArgumentNullException">userInfo should not be null</exception>
        public static void ReceivedRemoteNotification(this IAptkAmaNotificationService notificationService, NSDictionary userInfo)
        {
            if (userInfo == null)
                throw new ArgumentNullException(nameof(userInfo));

            if (!userInfo.ContainsKey(new NSString("aps")) 
                && userInfo.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey)
                && (userInfo = userInfo.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey) as NSDictionary) == null)
                return;

            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if (platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");

            var notification = (userInfo.ObjectForKey(new NSString("aps")) as NSDictionary)?.ToDictionary(n => n.Key.ToString(), n => n.Value.ToObject()).ToNotification();
            if (notification == null)
                return;
            
            Debug.WriteLine($"Notification received: {JsonConvert.SerializeObject(notification)}");

            UIApplication.SharedApplication.InvokeOnMainThread(() => platformNotificationService.Configuration.NotificationHandler?.OnNotificationReceived(notification));
        }

        /// <summary>
        /// Call this method manualy to unregister from notifications by yourself from platform
        /// </summary>
        /// <param name="notificationService">This notification service</param>
        /// <param name="deviceToken">The token of the device provided by the platform</param>
        /// <exception cref="ArgumentNullException">deviceToken should not be null</exception>
        public static void Unregister(this IAptkAmaNotificationService notificationService, NSData deviceToken)
        {
            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if (platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");

            if (platformNotificationService.Tcs == null)
                return;

            if (deviceToken == null)
            {
                var deviceTokenValue = string.Empty;
                platformNotificationService.Configuration.CacheService?.TryLoadRegistrationId(out deviceTokenValue);
                
                if(string.IsNullOrEmpty(deviceTokenValue))
                    throw new ArgumentNullException(nameof(deviceToken));

                deviceToken = NSData.FromString(deviceTokenValue);
            }
            
            try
            {
                platformNotificationService.DeviceToken = deviceToken;
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();

                if (platformNotificationService.PendingUnregistrations != null && platformNotificationService.PendingUnregistrations.Any())
                {
                    foreach (var pendingUnregistration in platformNotificationService.PendingUnregistrations)
                    {
                        UIApplication.SharedApplication.InvokeOnMainThread(async () =>
                        {
                            await push.UnregisterTemplateAsync(pendingUnregistration.Name);

                            platformNotificationService.Configuration.NotificationHandler?.OnUnregistrationSucceeded(pendingUnregistration.Name);
                        });
                    } 
                }
                else
                {
                    UIApplication.SharedApplication.InvokeOnMainThread(async () =>
                    {
                        await push.UnregisterAllAsync(deviceToken);

                        platformNotificationService.Configuration.NotificationHandler?.OnUnregistrationSucceeded("All");
                    });
                }

                UIApplication.SharedApplication.InvokeOnMainThread(() => platformNotificationService.Configuration.CacheService?.ClearRegistrationId());
                platformNotificationService.Tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                platformNotificationService.Tcs.SetException(ex);
            }
            finally
            {
                platformNotificationService.PendingUnregistrations = null;
                platformNotificationService.Tcs = null;
            }
        }
    }
}
