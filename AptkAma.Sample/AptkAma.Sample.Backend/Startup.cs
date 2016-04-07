using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AptkAma.Sample.Backend.Startup))]

namespace AptkAma.Sample.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}