using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Json;
using Castle.Facilities.Logging;
using Hangfire;
using Hangfire.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using SoapCore;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Wimi.BtlCore.Authentication.JwtBearer;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Identity;
using Wimi.BtlCore.Web.Resources;
using Wimi.BtlCore.WebServices;

namespace Wimi.BtlCore.Web.Startup
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;
        readonly string _defaultCorsPolicyName = "localhost";

        public Startup(IWebHostEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // MVC
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddScoped<IWebResourceManager, WebResourceManager>();

            services.AddSignalR();

            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                ));

            services.AddHttpContextAccessor();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddHangfire(
                r =>
                {
                    r.UseSqlServerStorage(AppSettings.Database.ConnectionString);
                    r.UseLogProvider(new CustomLogProvider());
                });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 5000; // 5000 items max
                options.ValueLengthLimit = 1024 * 1024 * 100; // 100MB max len form data
            });

            services.AddHangfireServer();
            services.AddSoapCore();

            // Configure Abp and Dependency Injection               // Configure Log4Net logging
            return services.AddAbp<BtlCoreWebMvcModule>(options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseAbpLog4Net().WithConfig("log4net.config")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); // Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseJwtTokenMiddleware();

            app.UseAuthorization();

            //Use FastReport
            app.UseFastReport();

            app.UseCors(_defaultCorsPolicyName);

            
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                DashboardTitle = "作业调度平台"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");

            });

            // Create Soap 1.1 Bindingchuf

            // Create Soap 1.2 Binding
            //var textBindingElement = new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8);
            //var httpBindingElement = new HttpTransportBindingElement();
            //var binding = new CustomBinding(textBindingElement, httpBindingElement);

            app.UseSoapEndpoint<IWebServiceAppService>("/WimiWebService.asmx", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            //app.UseSoapEndpoint<IWebServiceAppService>("/WimiWebService.asmx", binding, SoapSerializer.DataContractSerializer);

            app.UseMiddleware<IgnoreRouteMiddleware>();
        }

        public class IgnoreRouteMiddleware
        {
            private readonly RequestDelegate next;

            public IgnoreRouteMiddleware(RequestDelegate next)
            {
                this.next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.Request.Path.HasValue && context.Request.Path.StartsWithSegments("/vision"))
                {
                    context.Response.StatusCode = 200;
                    context.Response.Redirect("/vision/index.html");
                    return;
                }

                //if (context.Request.Path.HasValue && context.Request.Path.StartsWithSegments("/TestVue"))
                //{
                //    context.Response.StatusCode = 200;
                //    context.Response.Redirect("/index.html");
                //    return;
                //}

                await next.Invoke(context);
            }
        }

    }

    public class CustomLogProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new CustomLogger { Name = name };
        }
    }

    public class CustomLogger : ILog
    {
        public string Name { get; set; }

        public bool Log(Hangfire.Logging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            if (messageFunc == null)
            {
                return logLevel > Hangfire.Logging.LogLevel.Debug;
            }
            return true;
        }
    }
}
