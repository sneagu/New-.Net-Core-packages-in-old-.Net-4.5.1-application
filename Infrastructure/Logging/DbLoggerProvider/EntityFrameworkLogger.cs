using System;
using System.Collections;
using System.Linq;
using System.Text;
//using Microsoft.AspNet.Http;
using System.Data.Entity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging.DbLoggerProvider
{
    public class EntityFrameworkLogger<TDbContext, TLog> : ILogger
        where TLog : Log, new()
        where TDbContext : DbContext
    {
        //const int _indentation = 2;
        readonly string _name;
        readonly Func<string, LogLevel, bool> _filter;
        //readonly IServiceProvider _serviceProvider;
        string _nameOrConnectionString;

        public EntityFrameworkLogger(string name, Func<string, LogLevel, bool> filter, string nameOrConnectionString/*IServiceProvider serviceProvider*/)
        {
            //if (serviceProvider == null)
            //{
            //    throw new ArgumentNullException(nameof(serviceProvider));
            //}
            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                throw new ArgumentNullException("nameOrConnectionString");
            }

            _name = name ?? string.Empty;
            _filter = filter ?? ((category, logLevel) => true);// ?? GetFilter(serviceProvider.GetService<IOptions<EntityFrameworkLoggerOptions>>());
            // _serviceProvider = serviceProvider;
            _nameOrConnectionString = nameOrConnectionString;
        }

        //private Func<string, LogLevel, bool> GetFilter(IOptions<EntityFrameworkLoggerOptions> options)
        //{
        //    if (options != null)
        //    {
        //        return ((category, level) => GetFilter(options.Value, category, level));
        //    }
        //    else
        //        return ((category, level) => true);
        //}

        //private bool GetFilter(EntityFrameworkLoggerOptions options, string category, LogLevel level)
        //{
        //    if (options.Filters != null)
        //    {
        //        var filter = options.Filters.Keys.FirstOrDefault(p => category.StartsWith(p));
        //        if (filter != null)
        //            return (int)options.Filters[filter] <= (int)level;
        //        else return true;
        //    }
        //    return true;
        //}

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null) || _filter(_name, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null)
            {
                return;
            }

            message = $"{message}";

            //if (exception != null)
            //{
            //    message += $"{Environment.NewLine}{Environment.NewLine}{exception}";
            //}

            WriteMessage(message, logLevel, eventId.Id, exception);
        }

        protected virtual void WriteMessage(string message, LogLevel logLevel, int eventId, Exception exception)
        {
            // create separate context for adding log
            //using (var context = ActivatorUtilities.CreateInstance<TDbContext>(_serviceProvider))
            using (var context = new LoggingDbContext(_nameOrConnectionString))
            {
                // create new log with resolving dependency injection
                //TLog log = this.Creator((int)logLevel, eventId, this.Name, message);

                //var log = ActivatorUtilities.CreateInstance<TLog>(_serviceProvider);
                //log.TimeStamp = DateTimeOffset.Now;
                //log.Level = logLevel;
                //log.EventId = eventId;
                //log.Name = logName.Length > 255 ? logName.Substring(0, 255) : logName;
                //log.Message = message;

                var log = new TLog
                {
                    Message = Trim(message, DbLoggerProvider.Log.MaximumMessageLength),
                    Date = DateTime.UtcNow,
                    Level = logLevel.ToString(),
                    Logger = Trim(_name, 255),
                    Thread = eventId.ToString()
                };

                if (exception != null)
                    log.Exception = Trim(exception.ToString(), DbLoggerProvider.Log.MaximumExceptionLength);

                // TODO: Get the username
                log.Username = System.Web.HttpContext.Current.User.ToString();

                context.Set<TLog>().Add(log);

                context.SaveChanges();
            }
        }

        private static string Trim(string value, int maximumLength)
        {
            return value.Length > maximumLength ? value.Substring(0, maximumLength) : value;
        }

        //public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        //{
        //    if (!IsEnabled(logLevel))
        //    {
        //        return;
        //    }
        //    var message = string.Empty;
        //    var values = state as ILogValues;
        //    if (formatter != null)
        //    {
        //        message = formatter(state, exception);
        //    }
        //    else if (values != null)
        //    {
        //        var builder = new StringBuilder();
        //        FormatLogValues(
        //            builder,
        //            values,
        //            level: 1,
        //            bullet: false);
        //        message = builder.ToString();
        //        if (exception != null)
        //        {
        //            message += Environment.NewLine + exception;
        //        }
        //    }
        //    else
        //    {
        //        message = LogFormatter.Formatter(state, exception);
        //    }
        //    if (string.IsNullOrEmpty(message))
        //    {
        //        return;
        //    }

        //    var log = new TLog
        //    {
        //        Message = Trim(message, DbLoggerProvider.Log.MaximumMessageLength),
        //        Date = DateTime.UtcNow,
        //        Level = logLevel.ToString(),
        //        Logger = _name,
        //        Thread = eventId.ToString()
        //    };

        //    if (exception != null)
        //        log.Exception = Trim(exception.ToString(), DbLoggerProvider.Log.MaximumExceptionLength);

        //    var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

        //    if (httpContext != null)
        //    {
        //        log.Browser = httpContext.Request.Headers["User-Agent"];
        //        log.Username = httpContext.User.Identity.Name;
        //        try { log.HostAddress = httpContext.Connection.LocalIpAddress?.ToString(); }
        //        catch (ObjectDisposedException) { log.HostAddress = "Disposed"; }
        //        log.Url = httpContext.Request.Path;
        //    }

        //    var db = _serviceProvider.GetRequiredService<TDbContext>();
        //    db.Set<TLog>().Add(log);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        System.Console.WriteLine(e.Message);
        //    }

        //}

        //public IDisposable BeginScopeImpl(object state)
        //{
        //    return new NoopDisposable();
        //}

        //private void FormatLogValues(StringBuilder builder, ILogValues logValues, int level, bool bullet)
        //{
        //    var values = logValues.GetValues();
        //    if (values == null)
        //    {
        //        return;
        //    }
        //    var isFirst = true;
        //    foreach (var kvp in values)
        //    {
        //        builder.AppendLine();
        //        if (bullet && isFirst)
        //        {
        //            builder.Append(' ', level * _indentation - 1)
        //                   .Append('-');
        //        }
        //        else
        //        {
        //            builder.Append(' ', level * _indentation);
        //        }
        //        builder.Append(kvp.Key)
        //               .Append(": ");
        //        if (kvp.Value is IEnumerable && !(kvp.Value is string))
        //        {
        //            foreach (var value in (IEnumerable)kvp.Value)
        //            {
        //                if (value is ILogValues)
        //                {
        //                    FormatLogValues(
        //                        builder,
        //                        (ILogValues)value,
        //                        level + 1,
        //                        bullet: true);
        //                }
        //                else
        //                {
        //                    builder.AppendLine()
        //                           .Append(' ', (level + 1) * _indentation)
        //                           .Append(value);
        //                }
        //            }
        //        }
        //        else if (kvp.Value is ILogValues)
        //        {
        //            FormatLogValues(
        //                builder,
        //                (ILogValues)kvp.Value,
        //                level + 1,
        //                bullet: false);
        //        }
        //        else
        //        {
        //            builder.Append(kvp.Value);
        //        }
        //        isFirst = false;
        //    }
        //}

        private class NoopDisposable : IDisposable
        {
            /// <summary>
            /// The instance.
            /// </summary>
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }

}

