using System;
using Common;
using Autofac;
using AspNetCoreRateLimit;
using WebFramework.Swagger;
using WebFramework.Middlewares;
using WebFramework.CustomMapping;
using WebFramework.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using OwaspHeaders.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyApi
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _siteSetting = Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.InitializeAutoMapper();

            services.AddDbContext(Configuration, _siteSetting);

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            services.AddMinimalMvc();

            services.AddJwtAuthentication(_siteSetting.JwtSettings);

            services.AddCustomApiVersioning();

            services.AddCors();

            services.AddSwagger();

            services.AddRolePolicy();

            services.AddMemoryCache();

            services.AddOptions();

            services.AddCronJob();

            services.AddSecondLevelCache(_siteSetting);

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            //services.AddElmah(Configuration, _siteSetting);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacConfigurationExtensions());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIpRateLimiting();

            app.IntializeDatabase();

            app.UseCustomExceptionHandler();

            app.UseHsts(env);

            //app.UseElmah();

            app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareConfiguration.CustomConfiguration());

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSwaggerAndUi();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}