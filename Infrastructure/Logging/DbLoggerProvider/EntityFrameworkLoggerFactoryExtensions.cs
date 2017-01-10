using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Data.Entity;

namespace Infrastructure.Logging.DbLoggerProvider
{
    public static class EntityFrameworkLoggerFactoryExtensions
    {
        public static ILoggerFactory AddEntityFramework<TDbContext, TLog>(this ILoggerFactory factory, string nameOrConnectionString/*IServiceProvider serviceProvider*/
            , Func<string, LogLevel, bool> filter = null)
            where TDbContext : DbContext
            where TLog : Log, new()
        {
            if (factory == null) throw new ArgumentNullException("factory");

            factory.AddProvider(new EntityFrameworkLoggerProvider<TDbContext, TLog>(nameOrConnectionString/*serviceProvider*/, filter));

            return factory;
        }

        public static ILoggerFactory AddEntityFramework<TDbContext, TLog>(this ILoggerFactory factory, string nameOrConnectionString
            , IConfiguration configuration)
            where TDbContext : DbContext
            where TLog : Log, new()
        {
            if (factory == null) throw new ArgumentNullException("factory");

            var settings = new ConfigurationConsoleLoggerSettings(configuration);
            factory.AddProvider(new EntityFrameworkLoggerProvider<TDbContext, TLog>(nameOrConnectionString, settings));

            return factory;
        }
    }
}
