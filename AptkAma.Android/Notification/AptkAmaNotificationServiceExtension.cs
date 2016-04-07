using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Linq;
using Android.Runtime;
using Aptk.Plugins.AptkAma.Extensions;
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

            if (platformNotificationService.Tcs == null)
                return;

            try
            {
                platformNotificationService.RegistrationId = registrationId;
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();

                foreach (var pendingRegistration in platformNotificationService.PendingRegistrations)
                {
                    var template = JsonConvert.SerializeObject(new Dictionary<string, object> { { "data", pendingRegistration } });

                    Application.SynchronizationContext.Post(async _ =>
                    {
                        await push.RegisterTemplateAsync(registrationId, template, pendingRegistration.Name, pendingRegistration.Tags);

                        Debug.WriteLine($"Notification {pendingRegistration.Name} registered for device {registrationId} with template {template} and tags {pendingRegistration.Tags}");

                        platformNotificationService.Configuration.NotificationHandler?.OnRegistrationSucceeded(pendingRegistration.Name);
                    }, null);
                }

                Application.SynchronizationContext.Post(_ => platformNotificationService.Configuration.CacheService?.SaveRegistrationId(registrationId), null);
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

            if (platformNotificationService.Tcs == null)
                return;

            try
            {
                platformNotificationService.RegistrationId = registrationId;
                var push = ((MobileServiceClient)platformNotificationService.Client).GetPush();

                if (platformNotificationService.PendingUnregistrations != null && platformNotificationService.PendingUnregistrations.Any())
                {
                    foreach (var pendingUnregistration in platformNotificationService.PendingUnregistrations)
                    {
                        Application.SynchronizationContext.Post(async _ =>
                        {
                            await push.UnregisterTemplateAsync(pendingUnregistration.Name);

                            platformNotificationService.Configuration.NotificationHandler?.OnUnregistrationSucceeded(pendingUnregistration.Name);
                        }, null);
                    }
                }
                else
                {
                    Application.SynchronizationContext.Post(async _ =>
                    {
                        await push.UnregisterAllAsync(registrationId);

                        platformNotificationService.Configuration.NotificationHandler?.OnUnregistrationSucceeded("All");
                    }, null);
                }

                Application.SynchronizationContext.Post(_ => platformNotificationService.Configuration.CacheService?.ClearRegistrationId(), null);
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

        internal static void OnError(this IAptkAmaNotificationService notificationService, Context context, string errorId)
        {
            if (errorId == null)
                throw new ArgumentNullException(nameof(errorId));

            var platformNotificationService = notificationService as AptkAmaNotificationService;
            if (platformNotificationService == null)
                throw new InvalidCastException("Unable to convert this generic instance into a platform specific one");

            if (platformNotificationService.Tcs == null)
                return;

            var errorMessage = $"Notification error code:  {errorId}";
            Debug.WriteLine(errorMessage);
            platformNotificationService.Tcs.SetException(new Exception(errorMessage));
            platformNotificationService.PendingRegistrations = null;
            platformNotificationService.Tcs = null;
            Application.SynchronizationContext.Post(_ =>
                {
                    platformNotificationService.Configuration.CacheService?.ClearRegistrationId();
                    platformNotificationService.Configuration.NotificationHandler?.OnRegistrationFailed(errorMessage);
                }, null);
    }
    }
}