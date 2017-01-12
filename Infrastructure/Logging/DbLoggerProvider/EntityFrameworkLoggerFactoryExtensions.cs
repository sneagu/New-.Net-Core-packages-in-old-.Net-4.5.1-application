using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Data.Entity;

namespace Infrastructure.Logging.DbLoggerProvider
{
    public static class EntityFrameworkLoggerFactoryExtensions
    {
        public static ILoggerFactory AddEntityFramework(this ILoggerFactory factory, string nameOrConnectionString, Func<string, LogLevel, bool> filter = null)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                throw new ArgumentNullException("nameOrConnectionString");
            }


            factory.AddProvider(new EntityFrameworkLoggerProvider(nameOrConnectionString, filter));

            return factory;
        }

        public static ILoggerFactory AddEntityFramework(this ILoggerFactory factory, string nameOrConnectionString, LogLevel minLevel)
        {
            return AddEntityFramework(factory, nameOrConnectionString, (_, logLevel) => logLevel >= minLevel);
        }

        public static ILoggerFactory AddEntityFramework(this ILoggerFactory factory, string nameOrConnectionString, IConfiguration configuration)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                throw new ArgumentNullException("nameOrConnectionString");
            }

            var settings = new ConfigurationConsoleLoggerSettings(configuration);
            factory.AddProvider(new EntityFrameworkLoggerProvider(nameOrConnectionString, settings));

            return factory;
        }
    }
}
