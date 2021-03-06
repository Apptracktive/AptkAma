﻿using System;
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
using Newtonsoft.Json.Linq;
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

            if (platformNotificationService.Tcs == null ||
                platformNotificationService.Tcs.Task.IsCanceled ||
                platformNotificationService.Tcs.Task.IsCompleted ||
                platformNotificationService.Tcs.Task.IsFaulted)
                throw new Exception("Notification Task should not be null or terminated. Please open an issue on GitHub");

            try
            {
                platformNotificationService.DeviceToken = deviceToken;
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();

                var templates = new JObject();
                foreach (var pendingRegistration in platformNotificationService.PendingRegistrations)
                {
                    var template = JsonConvert.SerializeObject(new Dictionary<string, object> { {"aps", pendingRegistration} });
                    var body = new JObject
                    {
                        ["body"] = template
                    };

                    if (pendingRegistration.Tags != null)
                    {
                        var tags = JsonConvert.SerializeObject(pendingRegistration.Tags);

                        if (tags != null)
                            body.Add("tags", tags);
                    }

                    templates.Add(pendingRegistration.Name, body);
                }
                platformNotificationService.PendingRegistrations = null;

                UIApplication.SharedApplication.InvokeOnMainThread(async () =>
                {
                    await push.RegisterAsync(platformNotificationService.DeviceToken, templates);

                    Debug.WriteLine($"{templates} notifications registered");

                    platformNotificationService.Configuration.NotificationHandler?.OnRegistrationSucceeded();
                    platformNotificationService.Configuration.CacheService?.SaveRegistrationId(deviceToken.ToString());
                    platformNotificationService.Tcs.SetResult(true);
                });
            }
            catch (Exception ex)
            {
                platformNotificationService.Tcs.SetException(ex);
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

            if (platformNotificationService.Tcs == null ||
                platformNotificationService.Tcs.Task.IsCanceled ||
                platformNotificationService.Tcs.Task.IsCompleted ||
                platformNotificationService.Tcs.Task.IsFaulted)
                throw new Exception("Notification Task should not be null or terminated. Please open an issue on GitHub");

            Exception exception = null;
            try
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Error Code:  {0}\r\n", error.Code);
                sb.AppendFormat("Description: {0}\r\n", error.LocalizedDescription);
                var userInfo = error.UserInfo;
                for (int i = 0; i < userInfo.Keys.Length; i++)
                {
                    sb.AppendFormat("[{0}]: {1}\r\n", userInfo.Keys[i], userInfo.Values[i]);
                }
                exception = new Exception(sb.ToString());

                platformNotificationService.PendingRegistrations = null;
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    platformNotificationService.Configuration.CacheService?.ClearRegistrationId();
                    platformNotificationService.Configuration.NotificationHandler?.OnRegistrationFailed(sb.ToString());
                });
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                platformNotificationService.Tcs.SetException(exception);
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
            if (platformNotificationService?.Configuration?.NotificationHandler == null)
                throw new NullReferenceException("NotificationHandler could not be null when working with notifications");

            var notification = (userInfo.ObjectForKey(new NSString("aps")) as NSDictionary)?.ToDictionary(n => n.Key.ToString(), n => n.Value.ToObject()).ToNotification();
            if (notification == null)
            {
                Debug.WriteLine($"Notification convertion failed for: {userInfo}");
                return;
            }

            Debug.WriteLine($"Notification received: {JsonConvert.SerializeObject(notification)}");

            UIApplication.SharedApplication.InvokeOnMainThread(() => platformNotificationService.Configuration.NotificationHandler.OnNotificationReceived(notification));
        }

        /// <summary>
        /// Call this method manualy to unregister from notifications by yourself from platform
        /// </summary>
        /// <param name="notificationService">This notification service</param>
        /// <exception cref="ArgumentNullException">deviceToken should not be null</exception>
        public static void Unregister(this IAptkAmaNotificationService notificationService)
        {
            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if (platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");
            
            if (platformNotificationService.Tcs == null ||
                platformNotificationService.Tcs.Task.IsCanceled ||
                platformNotificationService.Tcs.Task.IsCompleted ||
                platformNotificationService.Tcs.Task.IsFaulted)
                throw new Exception("Notification Task should not be null or terminated. Please open an issue on GitHub");

            try
            {
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();
                
                UIApplication.SharedApplication.InvokeOnMainThread(async () =>
                {
                    await push.UnregisterAsync();

                    platformNotificationService.Configuration.NotificationHandler?.OnUnregistrationSucceeded();
                    platformNotificationService.Configuration.CacheService?.ClearRegistrationId();
                    platformNotificationService.Tcs.SetResult(true);
                });
            }
            catch (Exception ex)
            {
                platformNotificationService.Tcs.SetException(ex);
            }
        }
    }
}
