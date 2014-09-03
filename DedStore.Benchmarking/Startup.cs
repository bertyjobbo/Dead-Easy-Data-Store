using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DedStore.Benchmarking.Startup))]
namespace DedStore.Benchmarking
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
