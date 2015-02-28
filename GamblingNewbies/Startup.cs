using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GamblingNewbies.Startup))]
namespace GamblingNewbies
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
