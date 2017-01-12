using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class MailService : IMailService
    {
        private IConfigurationRoot _config;
        private ILogger<MailService> _logger;

        public MailService(IConfigurationRoot config, ILogger<MailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendMail( /*string template, string name, string email, string subject,*/ string msg)
        {
            //var x = new HttpClient();
            _logger.LogError("Email sent: ", msg);
        }
    }
}
