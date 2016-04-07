using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Networking.PushNotifications;
using Windows.UI.Core;
using Aptk.Plugins.AptkAma.Extensions;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Service to manage Push Notifications
    /// </summary>
    public class AptkAmaNotificationService : AptkAmaBaseNotificationService
    {
        private PushNotificationChannel _currentChannel;
        private CoreDispatcher _dispatcher;

        internal AptkAmaNotificationService(IMobileServiceClient client, IAptkAmaPluginConfiguration configuration) : base(client, configuration)
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        public override async Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications)
        {
            if (_currentChannel == null)
            {
                _currentChannel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                _currentChannel.PushNotificationReceived += OnPushNotificationReceived;
            }

            var push = ((MobileServiceClient)Client).GetPush();

            // Register for notifications using the new channel
            foreach (var notification in notifications)
            {
                var template = new XElement("toast",
                        new XElement("visual",
                            new XElement("binding", new XAttribute("template", notification.Name),
                                notification.Select(kv => new XElement(kv.Key, kv.Value)))));
                
                await push.RegisterTemplateAsync(_currentChannel.Uri, template.ToString(), notification.Name);

                Debug.WriteLine($"Notification {notification.Name} registered with template {template} and tags {notification.Tags}");

                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Configuration.NotificationHandler?.OnRegistrationSucceeded(notification.Name));
            }

            return true;
        }

        private async void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            if (args.NotificationType != PushNotificationType.Toast || args.ToastNotification == null)
                return;
            
            var xmlNotification = XDocument.Parse(args.ToastNotification.Content.GetXml());
            var notification = xmlNotification.Descendants("binding").Descendants().ToDictionary(n => n.Name.LocalName, n => n.Value as object).ToNotification();
            if (notification == null)
                return;

            args.Cancel = true;

            Debug.WriteLine($"Notification received: {JsonConvert.SerializeObject(notification)}");

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Configuration.NotificationHandler?.OnNotificationReceived(notification));
        }

        public override async Task<bool> UnregisterAsync(IEnumerable<IAptkAmaNotificationTemplate> notifications)
        {
            var push = ((MobileServiceClient)Client).GetPush();

            var pendingUnregistrations = notifications as IList<IAptkAmaNotificationTemplate> ?? notifications.ToList();
            if (notifications != null && pendingUnregistrations.Any())
            {
                foreach (var pendingUnregistration in pendingUnregistrations)
                {
                    await push.UnregisterTemplateAsync(pendingUnregistration.Name);

                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Configuration.NotificationHandler?.OnUnregistrationSucceeded(pendingUnregistration.Name));
                }
            }
            else
            {
                if (_currentChannel != null)
                {
                    await push.UnregisterAllAsync(_currentChannel.Uri);

                    _currentChannel.PushNotificationReceived -= OnPushNotificationReceived;
                    _currentChannel.Close();
                    _currentChannel = null;

                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Configuration.NotificationHandler?.OnUnregistrationSucceeded("All"));
                }
            }

            return true;
        }
    }
}
