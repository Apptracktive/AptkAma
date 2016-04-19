using System.Net.Http;
using Android.App;
using Android.OS;
using Aptk.Plugins.AptkAma;
using Aptk.Plugins.AptkAma.Data;
using Aptk.Plugins.AptkAma.Identity;
using AptkAma.Sample.Core;
using AptkAma.Sample.Core.Helpers;
using AptkAma.Sample.Core.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace AptkAma.Sample.Droid
{
    [Activity(Label = "AptkAma", MainLauncher = true, Icon = "@drawable/logo_rvb_72")]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            InitAptkAmaPlugin();

            this.LoadApplication(new App());
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
            AptkAmaPluginLoader.Init(configuration, ApplicationContext);

            // [Optional] If AptkAmaIdentityHandler is used, give it an instance of the plugin after Init
            identityHandler.AptkAmaService = AptkAmaPluginLoader.Instance;
            
            // Init local store extension
            AptkAmaLocalStorePluginLoader.Init(new AptkAmaLocalStorePluginConfiguration(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)));
        }
    }
}

