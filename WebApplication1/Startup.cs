using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.Options;
using Infrastructure.Configuration.ResxConfigProvider;
using Microsoft.Extensions.Logging;
using Infrastructure.Configuration;
using Infrastructure.Configuration.DbConfigProvider;
using Infrastructure.Logging.DbLoggerProvider;
using Infrastructure.Logging.EmailLoggerProvider;

[assembly: OwinStartupAttribute(typeof(WebApplication1.Startup))]
namespace WebApplication1
{
    public partial class Startup
    {
        private IConfigurationRoot _config;
        private IServiceProvider _serviceProvider;

        public void Configuration(IAppBuilder app)
        {
            var services = new ServiceCollection();
            ConfigureAuth(app);
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            // http://scottdorman.github.io/2016/03/17/integrating-asp.net-core-dependency-injection-in-mvc-4/
            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
               .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
               .Where(t => typeof(IController).IsAssignableFrom(t)
                  || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));

            services.AddOptions();
            services.AddLogging();
            _config = services.AddConfiguration();
            services.AddAppDependencies();

            _serviceProvider = services.BuildServiceProvider();
            _serviceProvider.AddLogging(_config);

            var resolver = new DefaultDependencyResolver(_serviceProvider);
            DependencyResolver.SetResolver(resolver);
        }
    }

    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider serviceProvider;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.serviceProvider.GetServices(serviceType);
        }
    }

    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddControllersAsServices(this IServiceCollection services,
           IEnumerable<Type> controllerTypes)
        {
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }

        public static IConfigurationRoot AddConfiguration(this IServiceCollection services)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "username", "Guest" }
                })
                //.AddConfigFile(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
                .AddJsonFile(@"App_Data\config.json")
                .AddJsonFile(@"App_Data\appsettings.json")
                .AddXmlFile(@"App_Data\appsettings.xml")
                .AddEntityFrameworkConfig(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
                .AddResxFile(@"App_Data\Resource1.resx")
                .AddEnvironmentVariables()
                .Build();

            // Options (see config.json)
            services.Configure<MySettings>(mySettings =>
            {
                mySettings.DateSetting = DateTime.Today;
            });

            services.Configure<MySettings>(configuration);
            //services.AddSingleton<IConfigureOptions<MySettings>>(new ConfigureFromConfigurationOptions<MySettings>(configuration));

            services.Configure<OtherSettings>(configuration.GetSection("otherSettings"));


            // *If* you need access to generic IConfiguration this is **required**
            services.AddSingleton<IConfigurationRoot>(configuration);

            //// Logging
            //// Note: WithFilter has priority over configuration settings
            //ILoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory
            //    .WithFilter(
            //        new FilterLoggerSettings
            //        {
            //            { "Microsoft", LogLevel.None },
            //            { "System", LogLevel.None },
            //            { "WebApplication1.Controllers.HomeController", LogLevel.Information }
            //        })
            //    .AddConsole(configuration.GetSection("Logging"))
            //    //.AddDebug()
            //    //.AddEntityFramework(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString) // filter = null
            //    //.AddEntityFramework(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, (_, logLevel) => logLevel >= LogLevel.Error);
            //    .AddEntityFramework(configuration["ConnectionStrings:DefaultConnection"], configuration.GetSection("Logging"))
            //    .AddEmail(/*mailService, */LogLevel.Critical); // TODO: How to inject email service?            
            //services.AddSingleton(loggerFactory); // Add first my already configured instance
            //services.AddLogging(); // Allow ILogger<T>

            return configuration;
        }

        public static IServiceProvider AddLogging(this IServiceProvider serviceProvider, IConfigurationRoot configuration)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var mailService = serviceProvider.GetService<IMailService>();
            loggerFactory
                .WithFilter(
                    new FilterLoggerSettings
                    {
                        { "Microsoft", LogLevel.None },
                        { "System", LogLevel.None },
                        { "WebApplication1.Controllers.HomeController", LogLevel.Information }
                    })
                .AddConsole(configuration.GetSection("Logging"))
                .AddDebug()
                //.AddEntityFramework(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString) // filter = null
                //.AddEntityFramework(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, (_, logLevel) => logLevel >= LogLevel.Error);
                .AddEntityFramework(configuration["ConnectionStrings:DefaultConnection"], configuration.GetSection("Logging"))
                .AddEmail(mailService, LogLevel.Critical); // TODO: How to inject email service?  

            return serviceProvider;
        }

        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.AddTransient<IMailService, MailService>();

            return services;
        }
    }

}
