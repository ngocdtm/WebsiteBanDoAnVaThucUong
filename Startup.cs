using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebsiteBanDoAnVaThucUong.Startup))]
namespace WebsiteBanDoAnVaThucUong
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
