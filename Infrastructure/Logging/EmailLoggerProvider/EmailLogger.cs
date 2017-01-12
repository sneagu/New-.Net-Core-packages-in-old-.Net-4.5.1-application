using System;
//using Microsoft.AspNetCore.Http;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging.EmailLoggerProvider
{
    public class EmailLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private IMailService _mailService;

        public EmailLogger(string categoryName, Func<string, LogLevel, bool> filter, IMailService mailService)
        {
            _categoryName = categoryName;
            _filter = filter;
            _mailService = mailService;
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

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = string.Format("Level: {0} {1}", logLevel, message);

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            _mailService.SendMail(/*"logmessage.txt", "Shawn Wildermuth", "shawn@wildermuth.com", "[WilderBlog Log Message]", */message);

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // Not necessary
            return null;
        }
    }
}
