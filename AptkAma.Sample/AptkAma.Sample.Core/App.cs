using Aptk.Plugins.AptkAma;
using AptkAma.Sample.Core.Helpers;
using Xamarin.Forms;

namespace AptkAma.Sample.Core
{
    public class App : Application
    {
        private readonly IAptkAmaService _aptkAmaService;

        public App()
        {
            this.MainPage = new MainPage();
            _aptkAmaService = AptkAmaPluginLoader.Instance;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            //await _aptkAmaService.Notification.UnregisterAllAsync();
            var success = await _aptkAmaService.Notification.RegisterAsync(AptkAmaNotificationHandler.TestNotificationTemplate);
        }
    }
}
