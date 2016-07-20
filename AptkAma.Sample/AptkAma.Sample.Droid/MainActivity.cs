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

            this.LoadApplication(new App());
        }
    }
}

