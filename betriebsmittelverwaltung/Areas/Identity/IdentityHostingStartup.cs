using System;
using betriebsmittelverwaltung.Areas.Identity.Data;
using betriebsmittelverwaltung.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(betriebsmittelverwaltung.Areas.Identity.IdentityHostingStartup))]
namespace betriebsmittelverwaltung.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AppDBContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AppDBContextConnection")));

                services.AddDefaultIdentity<User>()
                    .AddEntityFrameworkStores<AppDBContext>();
            });
        }
    }
}