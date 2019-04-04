using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HooliganGame.Startup))]
namespace HooliganGame
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
