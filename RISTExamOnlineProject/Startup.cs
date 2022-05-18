﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RISTExamOnlineProject.Hubs;
using RISTExamOnlineProject.Models.db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;

namespace RISTExamOnlineProject
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

         

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var constr = Configuration.GetConnectionString("CONSPTO");
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<SPTODbContext>(options =>
                    options.UseSqlServer(constr));

         
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowCredentials().Build();
            }));
            
            services.AddSignalR();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //services.AddMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Test.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
             
           

            
            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();
           
            app.UseCookiePolicy();
            app.UseAuthentication();
           
            app.UseCors("CorsPolicy");


            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Account}/{action=Login}");
            });

            app.UseSignalR(routes => { routes.MapHub<CounterHub>("/CounterHub"); });


            app.UseFastReport();
        }
    }
}