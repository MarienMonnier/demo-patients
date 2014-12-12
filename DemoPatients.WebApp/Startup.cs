using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(DemoPatients.WebApp.Startup))]
namespace DemoPatients.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}