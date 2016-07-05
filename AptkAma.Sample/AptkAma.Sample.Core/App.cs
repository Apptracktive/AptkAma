using System.Net.Http;
using Aptk.Plugins.AptkAma;
using Aptk.Plugins.AptkAma.Data;
using Aptk.Plugins.AptkAma.Identity;
using AptkAma.Sample.Core.Helpers;
using AptkAma.Sample.Core.Services;
using Xamarin.Forms;

namespace AptkAma.Sample.Core
{
    public class App : Application
    {
        private readonly IAptkAmaService _aptkAmaService;

        public App()
        {
            InitAptkAmaPlugin();

            _aptkAmaService = AptkAmaPluginLoader.Instance;

            this.MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            //await _aptkAmaService.Notification.UnregisterAsync();
            //var success = await _aptkAmaService.Notification.RegisterAsync(AptkAmaNotificationHandler.TestNotificationTemplate);
        }

        private void InitAptkAmaPlugin()
        {
            var configuration = new AptkAmaPluginConfiguration(Constants.AmsUrl, Constants.ModelAssembly);

            // [Optional] Handle expired token to automaticaly ask for login if needed
            var identityHandler = new AptkAmaIdentityHandler(configuration);
            configuration.HttpHandlers = new HttpMessageHandler[] { identityHandler };

            // [Optional] Manage local caching
            configuration.CacheService = new AptkAmaCacheService();

            // [Optional] Handle notifications
            configuration.NotificationHandler = new AptkAmaNotificationHandler();

            // Init main plugin
            AptkAmaPluginLoader.Init(configuration);

            // [Optional] If AptkAmaIdentityHandler is used, give it an instance of the plugin after Init
            identityHandler.AptkAmaService = AptkAmaPluginLoader.Instance;

            // Init local store extension
            AptkAmaLocalStorePluginLoader.Init(new AptkAmaLocalStorePluginConfiguration(AptkAmaFileStorePluginLoader.Instance));
        }
    }
}
