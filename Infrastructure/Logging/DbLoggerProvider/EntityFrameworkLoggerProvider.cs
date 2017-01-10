using Microsoft.Extensions.Logging;
using System;
using System.Data.Entity;
using Microsoft.Extensions.Logging.Console;
using System.Collections.Generic;

namespace Infrastructure.Logging.DbLoggerProvider
{
    public class EntityFrameworkLoggerProvider<TDbContext, TLog> : ILoggerProvider 
        where TLog : Log, new()
        where TDbContext : DbContext
    {
        readonly Func<string, LogLevel, bool> _filter;
        private IConsoleLoggerSettings _settings;
        //readonly IServiceProvider _serviceProvider;
        string _nameOrConnectionString;

        public EntityFrameworkLoggerProvider(string nameOrConnectionString, IConsoleLoggerSettings settings)
        {
            _nameOrConnectionString = nameOrConnectionString;
            _settings = settings;
        }

        public EntityFrameworkLoggerProvider(string nameOrConnectionString/*IServiceProvider serviceProvider*/, Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
            //_serviceProvider = serviceProvider;
            _nameOrConnectionString = nameOrConnectionString;

            _settings = new ConsoleLoggerSettings()
            {
                IncludeScopes = false,
            };
        }

        public ILogger CreateLogger(string name)
        {
            return new EntityFrameworkLogger<TDbContext, TLog>(name, GetFilter(name, _settings), _nameOrConnectionString /*_serviceProvider*/);
        }

        private Func<string, LogLevel, bool> GetFilter(string name, IConsoleLoggerSettings settings)
        {
            if (_filter != null)
            {
                return _filter;
            }

            if (settings != null)
            {
                foreach (var prefix in GetKeyPrefixes(name))
                {
                    LogLevel level;
                    if (settings.TryGetSwitch(prefix, out level))
                    {
                        return (n, l) => l >= level;
                    }
                }
            }

            return (n, l) => false;
        }

        private IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }

        public void Dispose() { }
    }
}
