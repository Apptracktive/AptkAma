using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using Aptk.Plugins.AptkAma.Extensions;
using Microsoft.Phone.Notification;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Service to manage Push Notifications
    /// </summary>
    public class AptkAmaNotificationService : AptkAmaBaseNotificationService
    {
        internal static HttpNotificationChannel CurrentChannel { get; private set; }

        public AptkAmaNotificationService(IMobileServiceClient client, IAptkAmaPluginConfiguration configuration) : base(client, configuration)
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

                CurrentChannel = HttpNotificationChannel.Find("AptkAmaPushChannel");

                if (CurrentChannel == null)
                {
                    CurrentChannel = new HttpNotificationChannel("AptkAmaPushChannel");
                    CurrentChannel.Open();
                    CurrentChannel.BindToShellToast();
                }

                CurrentChannel.ChannelUriUpdated += OnChannelUriUpdated;
                CurrentChannel.ErrorOccurred += OnErrorOccurred;
                CurrentChannel.ShellToastNotificationReceived += OnNotificationReceived;
            }
            return Tcs.Task;
        }

        private void OnChannelUriUpdated(object sender, NotificationChannelUriEventArgs args)
        {
            try
            {
                var push = ((MobileServiceClient)Client).GetPush();

                // Register for notifications using the new channel
                foreach (var notification in PendingRegistrations)
                {
                    XNamespace wp = "WPNotification";
                    var xmlTemplate = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), 
                        new XElement(wp + "Notification", new XAttribute(XNamespace.Xmlns + nameof(wp), wp),
                            new XElement(wp + "Toast", 
                                notification.Select(kv => new XElement(wp + kv.Key, kv.Value)))));
                    var template = $"{xmlTemplate.Declaration}{xmlTemplate}";

                    Deployment.Current.Dispatcher.BeginInvoke(async () =>
                    {
                        await push.RegisterTemplateAsync(CurrentChannel.ChannelUri.ToString(), template, notification.Name);

                        Debug.WriteLine($"Notification {notification.Name} registered with template {template} and tags {notification.Tags}");

                        Configuration.NotificationHandler?.OnRegistrationSucceeded(notification.Name);
                    });
                }

                Tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                Tcs.SetException(ex);
            }
            finally
            {
                PendingRegistrations = null;
                Tcs = null;
            }
        }

        private void OnErrorOccurred(object sender, NotificationChannelErrorEventArgs args)
        {
            var sb = new StringBuilder();
            try
            {
                sb.AppendFormat("Error Code:  {0}\r\n", args.ErrorCode);
                sb.AppendFormat("Description: {0}\r\n", args.Message);

                Tcs.SetException(new Exception(sb.ToString()));
            }
            catch (Exception ex)
            {
                Tcs.SetException(ex);
            }
            finally
            {
                PendingRegistrations = null;
                Tcs = null;
                Deployment.Current.Dispatcher.BeginInvoke(() => Configuration.NotificationHandler?.OnRegistrationFailed(sb.ToString()));
            }
        }

        private void OnNotificationReceived(object sender, NotificationEventArgs args)
        {
            var notification = args.Collection.ToDictionary(kv => kv.Key.Replace("wp:",string.Empty), kv => kv.Value as object).ToNotification();
            if(notification == null)
                return;

            Debug.WriteLine($"Notification received: {JsonConvert.SerializeObject(notification)}");

            Deployment.Current.Dispatcher.BeginInvoke(() => Configuration.NotificationHandler?.OnNotificationReceived(notification));
        }

        /// <summary>
        /// Unregister from Azure Push Notifications
        /// </summary>
        public override async Task<bool> UnregisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications)
        {
            var push = ((MobileServiceClient)Client).GetPush();

            var pendingUnregistrations = notifications as IList<IAptkAmaNotificationTemplate> ?? notifications.ToList();
            if (notifications != null && pendingUnregistrations.Any())
            {
                foreach (var pendingUnregistration in pendingUnregistrations)
                {
                    await push.UnregisterTemplateAsync(pendingUnregistration.Name);

                    Configuration.NotificationHandler?.OnUnregistrationSucceeded(pendingUnregistration.Name);
                }
            }
            else
            {
                CurrentChannel = CurrentChannel ?? HttpNotificationChannel.Find("AptkAmaPushChannel");

                if (CurrentChannel != null)
                {
                    await push.UnregisterAllAsync(CurrentChannel.ChannelUri.ToString());

                    CurrentChannel.ChannelUriUpdated -= OnChannelUriUpdated;
                    CurrentChannel.ErrorOccurred -= OnErrorOccurred;
                    CurrentChannel.ShellToastNotificationReceived -= OnNotificationReceived;
                    CurrentChannel.Close();
                    CurrentChannel.Dispose();
                    CurrentChannel = null;

                    Configuration.NotificationHandler?.OnUnregistrationSucceeded("All");
                }
            }

            return true;
        }
    }
}
