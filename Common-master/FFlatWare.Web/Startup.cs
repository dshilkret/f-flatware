using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFlatWare.Web
{
    public class Startup
    {
        /// <summary>the current environment</summary>
        public IWebHostEnvironment CurrentEnv { get; set; }

        public Startup( IConfiguration configuration, IWebHostEnvironment env )
        {
            // inject the configuration
            Configuration = configuration;

            // set the current environment for later use
            this.CurrentEnv = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddControllersWithViews();

            // this code is required if you enabled sessions in Configure
            //services.AddSession( options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromHours( 1 );
            //} );

            // handles hot reload of a running web site when used in DEV, remove in PROD
            // has a conditional nuget package reference on AspNetCore.Mvc.Razor.RuntimeCompilation
#if DEBUG
            if ( this.CurrentEnv.IsDevelopment() )
                services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            // if an unhandled exceptionis throw - then either show in development or go to the error view
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler( "/Home/Error" );

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // set the default and supported cultures
            var supportedCultures = new[] { "en-US" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture( supportedCultures[0] )
                .AddSupportedCultures( supportedCultures )
                .AddSupportedUICultures( supportedCultures );
            app.UseRequestLocalization( localizationOptions );

            // makes all HTTP requets go to HTTPS
            app.UseHttpsRedirection();

            // allows you to returen static files in the response
            app.UseStaticFiles();

            // turns on routing
            app.UseRouting();

            // enable sessions only if you want to track user via ASP.NET Sessions
            //app.UseSession();

            app.UseAuthorization();

            // specificy routing endpoints
            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}" );

            } );
        }
    }
}
