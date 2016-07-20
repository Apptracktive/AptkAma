using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Gcm;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Plugin.CurrentActivity;

namespace Aptk.Plugins.AptkAma.Notification
{
    public class AptkAmaNotificationService : AptkAmaBaseNotificationService
    {
        private bool _isInitialized;
        public string RegistrationId;

        public AptkAmaNotificationService(IMobileServiceClient client, IAptkAmaPluginConfiguration configuration) : base(client, configuration)
        {
        }

        private void Initialize()
        {
                GcmClient.CheckDevice(CrossCurrentActivity.Current.Activity);
                GcmClient.CheckManifest(CrossCurrentActivity.Current.Activity);
                _isInitialized = true;
        }

        /// <summary>
        /// Register to Azure Push Notifications
        /// </summary>
        /// <param name="templates">Notifications to register for</param>
        /// <param name="cancellationToken">Token to cancel registration</param>
        public override Task<bool> RegisterAsync(IEnumerable<IAptkAmaNotificationTemplate> templates, CancellationToken cancellationToken)
        {
            if (Tcs == null || Tcs.Task.IsCanceled || Tcs.Task.IsCompleted || Tcs.Task.IsFaulted)
            {
                Tcs = new TaskCompletionSource<bool>();
                cancellationToken.Register(() => Tcs.TrySetCanceled(), false);

                if (!_isInitialized)
                    Initialize();

                PendingRegistrations = templates;
                Debug.WriteLine($"Trying to register for notifications {JsonConvert.SerializeObject(Configuration.NotificationHandler?.GoogleSenderIds)}");
                GcmClient.Register(CrossCurrentActivity.Current.Activity, Configuration.NotificationHandler?.GoogleSenderIds);
            }
            return Tcs.Task;
        }

        /// <summary>
        /// Unregister from Azure Push Notifications
        /// </summary>
        /// <param name="cancellationToken">Token to cancel registration</param>
        public override Task<bool> UnregisterAsync(CancellationToken cancellationToken)
        {
            if (Tcs == null || Tcs.Task.IsCanceled || Tcs.Task.IsCompleted || Tcs.Task.IsFaulted)
            {
                Tcs = new TaskCompletionSource<bool>();
                cancellationToken.Register(() => Tcs.TrySetCanceled(), false);

                if (!_isInitialized)
                    Initialize();

                Debug.WriteLine($"Trying to unregister");
                GcmClient.UnRegister(CrossCurrentActivity.Current.Activity);
            }
            return Tcs.Task;
        }
    }
}