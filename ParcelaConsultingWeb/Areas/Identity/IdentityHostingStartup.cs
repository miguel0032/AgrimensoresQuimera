using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ParcelaConsultingWeb.Areas.Identity.IdentityHostingStartup))]
namespace ParcelaConsultingWeb.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}