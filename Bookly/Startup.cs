using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Bookly.Startup))]
namespace Bookly
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        
    }
}