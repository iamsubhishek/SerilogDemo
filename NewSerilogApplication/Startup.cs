using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewSerilogApplication.Interface;
using NewSerilogApplication.Models;
using Serilog;
using Serilog.Context;
using Serilog.Enrichers.AspNetCore;
using Serilog.Enrichers.AspnetcoreHttpcontext;
using UAParser;

namespace NewSerilogApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Read Configuration from appSettings
            //var config = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSingleton(typeof(ILog<>), typeof(LogInfo<>));
            services.AddHttpContextAccessor();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.Use(async (HttpContext, next) =>
            {
                //Get username  
                var username = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.Identity.Name : "anonymous";
                LogContext.PushProperty("User", username);

                //Get remote IP address  
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                LogContext.PushProperty("RemoteIP", !String.IsNullOrWhiteSpace(ip) ? ip : "unknown");

                //Get Raw URL
                var FullUrl = HttpContext.Request.GetDisplayUrl();
                LogContext.PushProperty("BrowserUrl", FullUrl);

                var RemotePort = HttpContext.Connection.RemotePort;
                LogContext.PushProperty("RemotePort", RemotePort);

                var ApplicationName = env.ApplicationName;
                LogContext.PushProperty("ApplicationName", ApplicationName);

                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                // get a parser with the embedded regex patterns
                var uaParser = Parser.GetDefault();
                ClientInfo c = uaParser.Parse(userAgent);
                var BrowserType = c.UA.Family;
                //var Device = c.Device.Family;
                var BrowserVersion = c.UA.Major + "." + c.UA.Minor + "." + c.UA.Patch;
                LogContext.PushProperty("BrowserName", BrowserType);
                LogContext.PushProperty("BrowserVersion", BrowserVersion);

                await next.Invoke();
            });
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
