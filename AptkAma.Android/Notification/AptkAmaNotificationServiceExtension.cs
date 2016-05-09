using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Linq;
using Android.Runtime;
using Aptk.Plugins.AptkAma.Extensions;
using Newtonsoft.Json.Linq;
using Debug = System.Diagnostics.Debug;

namespace Aptk.Plugins.AptkAma.Notification
{
    internal static class AptkAmaNotificationServiceExtension
    {
        internal static void OnRegistered(this IAptkAmaNotificationService notificationService, Context context, string registrationId)
        {
            if (string.IsNullOrEmpty(registrationId))
                throw new ArgumentNullException(nameof(registrationId));

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
                platformNotificationService.RegistrationId = registrationId;
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();

                var templates = new JObject();

                foreach (var pendingRegistration in platformNotificationService.PendingRegistrations)
                {
                    var template = JsonConvert.SerializeObject(new Dictionary<string, object> { { "data", pendingRegistration } });
                    var body = new JObject
                    {
                        ["body"] = template
                    };

                    if (pendingRegistration.Tags != null)
                    {
                        var tags = JsonConvert.SerializeObject(pendingRegistration.Tags);

                        if(tags != null)
                            body.Add("tags", tags);
                    }

                    templates.Add(pendingRegistration.Name, body);
                }
                platformNotificationService.PendingRegistrations = null;

                Application.SynchronizationContext.Post(async _ =>
                {
                    await push.RegisterAsync(registrationId, templates);

                    Debug.WriteLine($"{templates} notifications registered");

                    platformNotificationService.Configuration.NotificationHandler?.OnRegistrationSucceeded();
                    platformNotificationService.Configuration.CacheService?.SaveRegistrationId(registrationId);
                    platformNotificationService.Tcs.SetResult(true);
                }, null);

            }
            catch (Exception ex)
            {
                platformNotificationService.Tcs.SetException(ex);
            }
        }

        internal static void OnMessage(this IAptkAmaNotificationService notificationService, Context context, Intent intent)
        {
            if (intent == null)
                throw new ArgumentNullException(nameof(intent));

            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if (platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");

            var notification = intent.Extras.KeySet().Where(k => k != "from" && k != "collapse_key").ToDictionary<string, string, object>(key => key, key => intent.Extras.Get(key).ToString()).ToNotification();
            if (notification == null)
                return;

            Debug.WriteLine($"Notification received: {JsonConvert.SerializeObject(notification)}");

            Application.SynchronizationContext.Post(_ => platformNotificationService.Configuration.NotificationHandler?.OnNotificationReceived(notification), null);
        }

        internal static void OnUnRegistered(this IAptkAmaNotificationService notificationService, Context context, string registrationId)
        {
            if (string.IsNullOrEmpty(registrationId))
                throw new ArgumentNullException(nameof(registrationId));

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
                platformNotificationService.RegistrationId = registrationId;
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();

                Application.SynchronizationContext.Post(async _ =>
                {
                    await push.UnregisterAsync();
                    
                    platformNotificationService.Configuration.NotificationHandler?.OnUnregistrationSucceeded();
                    platformNotificationService.Configuration.CacheService?.ClearRegistrationId();
                    platformNotificationService.Tcs.SetResult(true);
                }, null);
            }
            catch (Exception ex)
            {
                platformNotificationService.Tcs.SetException(ex);
            }
        }

        internal static void OnError(this IAptkAmaNotificationService notificationService, Context context, string errorId)
        {
            if (errorId == null)
                throw new ArgumentNullException(nameof(errorId));

            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if (platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");

            if (platformNotificationService.Tcs == null ||
                platformNotificationService.Tcs.Task.IsCanceled ||
                platformNotificationService.Tcs.Task.IsCompleted ||
                platformNotificationService.Tcs.Task.IsFaulted)
                throw new Exception("Notification Task should not be null or terminated. Please open an issue on GitHub");

            var errorMessage = $"Notification error code:  {errorId}";
            Debug.WriteLine(errorMessage);
            Application.SynchronizationContext.Post(_ =>
            {
                platformNotificationService.PendingRegistrations = null;
                platformNotificationService.Configuration.CacheService?.ClearRegistrationId();
                platformNotificationService.Configuration.NotificationHandler?.OnRegistrationFailed(errorMessage);
                platformNotificationService.Tcs.SetException(new Exception(errorMessage));
            }, null);
        }
    }
}