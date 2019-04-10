using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QLBRUOU.Startup))]
namespace QLBRUOU
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
