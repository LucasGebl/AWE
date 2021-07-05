using betriebsmittelverwaltung.Areas.Identity.Data;
using betriebsmittelverwaltung.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace betriebsmittelverwaltung
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.Zero);
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;

            });

            //Identity
            services.AddDbContext<AppDBContext>(options =>
                  options.UseSqlServer(
                      Configuration.GetConnectionString("AppDBContextConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDBContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            SetupAdminUserAndRoles(userManager, roleManager).Wait();
            
        }

        private async Task SetupAdminUserAndRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            User user = await userManager.FindByNameAsync("admin@test.de");
            if (user == null)
            {
                user = new User() { Email = "admin@test.de", UserName = "admin@test.de"};
                await userManager.CreateAsync(user, "d9asdh93D89hd0_2h");
                System.Diagnostics.Debug.WriteLine(String.Format("User {0} was created", user.Email));
            }

            foreach (User.UserType ut in Enum.GetValues(typeof(User.UserType)))
            {
                IdentityRole role = await roleManager.FindByNameAsync(ut.ToString());
                if(role == null)
                {
                    role = new IdentityRole(ut.ToString());
                    await roleManager.CreateAsync(role);
                }
            }

            bool hasAdminRole = await userManager.IsInRoleAsync(user, "Admin");
            if (!hasAdminRole)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
