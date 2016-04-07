using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using Gcm;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace Aptk.Plugins.AptkAma.Notification
{
    public class AptkAmaNotificationService : AptkAmaBaseNotificationService
    {
        public readonly Context Context;
        private bool _isInitialized;
        public string RegistrationId;

        public AptkAmaNotificationService(IMobileServiceClient client, IAptkAmaPluginConfiguration configuration, Context context) : base(client, configuration)
        {
            Context = context;
        }

        private void Initialize()
        {
                GcmClient.CheckDevice(Context);
                GcmClient.CheckManifest(Context);
                _isInitialized = true;
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
                if (!_isInitialized)
                {
                    Initialize();
                }
                PendingRegistrations = notifications;
                Debug.WriteLine($"Trying to register for notifications {JsonConvert.SerializeObject(Configuration.NotificationHandler?.GoogleSenderIds)}");
                GcmClient.Register(Context, Configuration.NotificationHandler?.GoogleSenderIds);
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
                if (!_isInitialized)
                {
                    Initialize();
                }
                PendingUnregistrations = notifications;
                Debug.WriteLine($"Trying to unregister");
                GcmClient.UnRegister(Context);
            }
            return Tcs.Task;
        }
    }
}