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
using Newtonsoft.Json.Linq;

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

            var templates = new JObject();

            // Register for notifications using the new channel
            foreach (var notification in notifications)
            {
                var template = new XElement("toast",
                        new XElement("visual",
                            new XElement("binding", new XAttribute("template", notification.Name),
                                notification.Select(kv => new XElement(kv.Key, kv.Value)))));

                var body = new JObject
                {
                    ["body"] = template.ToString(),
                    ["headers"] = new JObject
                    {
                        ["X-WNS-Type"] = "wns/toast"
                    }
                };

                if (notification.Tags != null)
                {
                    var tags = JsonConvert.SerializeObject(notification.Tags);

                    if (tags != null)
                        body.Add("tags", tags);
                }

                templates.Add(notification.Name, body);
            }

            await push.RegisterAsync(_currentChannel.Uri, templates);

            Debug.WriteLine($"{notifications.Count()} notifications registered");

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Configuration.NotificationHandler?.OnRegistrationSucceeded());

            return true;
        }

        private async void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            if (args.NotificationType != PushNotificationType.Toast || args.ToastNotification == null)
                return;
            
            var xmlNotification = XDocument.Parse(args.ToastNotification.Content.GetXml());
            var notification = System.Xml.Linq.Extensions.Descendants(xmlNotification.Descendants("binding")).ToDictionary(n => n.Name.LocalName, n => n.Value as object).ToNotification();
            if (notification == null)
                return;

            args.Cancel = true;

            Debug.WriteLine($"Notification received: {JsonConvert.SerializeObject(notification)}");

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Configuration.NotificationHandler?.OnNotificationReceived(notification));
        }

        public override async Task<bool> UnregisterAsync()
        {
            var push = ((MobileServiceClient)Client).GetPush();

            
            if (_currentChannel != null)
            {
                await push.UnregisterAsync();

                _currentChannel.PushNotificationReceived -= OnPushNotificationReceived;
                _currentChannel.Close();
                _currentChannel = null;

                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Configuration.NotificationHandler?.OnUnregistrationSucceeded());
            }

            return true;
        }
    }
}
