using System;
//using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging.DbLoggerProvider
{
    public class EntityFrameworkLogger : ILogger
    {
        readonly string _name;
        readonly Func<string, LogLevel, bool> _filter;
        readonly string _nameOrConnectionString;

        public EntityFrameworkLogger(string name, Func<string, LogLevel, bool> filter, string nameOrConnectionString)
        {
            _name = name ?? string.Empty;
            _filter = filter ?? ((category, logLevel) => true);// ?? GetFilter(serviceProvider.GetService<IOptions<EntityFrameworkLoggerOptions>>());
            _nameOrConnectionString = nameOrConnectionString;
        }

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
                throw new ArgumentNullException("formatter");
            }

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null)
            {
                return;
            }

            //message = $"{message}";
            message = string.Format("{0}", message);

            //if (exception != null)
            //{
            //    message += $"{Environment.NewLine}{Environment.NewLine}{exception}";
            //}

            WriteMessage(message, logLevel, eventId.Id, exception);
        }

        protected virtual void WriteMessage(string message, LogLevel logLevel, int eventId, Exception exception)
        {
            // create separate context for adding log
            using (var context = new LoggingDbContext(_nameOrConnectionString))
            {
                var log = new Log
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
                //    var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

                //    if (httpContext != null)
                //    {
                //        log.Browser = httpContext.Request.Headers["User-Agent"];
                //        log.Username = httpContext.User.Identity.Name;
                //        try { log.HostAddress = httpContext.Connection.LocalIpAddress?.ToString(); }
                //        catch (ObjectDisposedException) { log.HostAddress = "Disposed"; }
                //        log.Url = httpContext.Request.Path;
                //    }
                log.Username = System.Web.HttpContext.Current.User.Identity.Name;

                context.Set<Log>().Add(log);

                context.SaveChanges();
            }
        }

        private static string Trim(string value, int maximumLength)
        {
            return value.Length > maximumLength ? value.Substring(0, maximumLength) : value;
        }

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

