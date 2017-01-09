using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Xunit;
using WebApplication1.Controllers;
using Microsoft.Extensions.Options;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Tests.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void Contact()
        {
            // Arrange
            var config = new ConfigurationRoot(new List<IConfigurationProvider> {new MemoryConfigurationProvider(new MemoryConfigurationSource())});
            config["kEY1"] = "keyValue1";
            config["key2"] = "keyValue2";
            config["USERNAME"] = "SNeagu";
            var otherSettings = new OtherSettings { Numbers = new int[] { 234, 567 } };
            var options = new OptionsWrapper<OtherSettings>(otherSettings);
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger<HomeController>();
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            HomeController controller = new HomeController(config, options, logger, cache);

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.Equal("keyValue1 SNeagu 234, 567", result.ViewBag.Message);
        }
    }
}
