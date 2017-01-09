using Microsoft.Extensions.Logging;
using System;
using System.Data.Entity;

namespace Infrastructure.Logging.DbLoggerProvider
{
    public class EntityFrameworkLoggerProvider<TDbContext, TLog> : ILoggerProvider 
        where TLog : Log, new()
        where TDbContext : DbContext
    {
        readonly Func<string, LogLevel, bool> _filter;
        //readonly IServiceProvider _serviceProvider;
        string _nameOrConnectionString;

        public EntityFrameworkLoggerProvider(string nameOrConnectionString/*IServiceProvider serviceProvider*/, Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
            //_serviceProvider = serviceProvider;
            _nameOrConnectionString = nameOrConnectionString;
        }

        public ILogger CreateLogger(string name)
        {
            return new EntityFrameworkLogger<TDbContext, TLog>(name, _filter,_nameOrConnectionString /*_serviceProvider*/);
        }

        public void Dispose() { }
    }
}
