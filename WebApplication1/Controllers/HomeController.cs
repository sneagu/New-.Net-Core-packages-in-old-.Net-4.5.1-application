using System;
using System.Web.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {

        IConfigurationRoot _configuration;

        OtherSettings _otherSettings;

        ILogger _logger;

        IMemoryCache _memoryCache;

        public HomeController(IConfigurationRoot configuration, IOptions<OtherSettings> otherSettings, ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _otherSettings = otherSettings.Value;
            _logger = logger;
            _memoryCache = memoryCache;
        }
        public ActionResult Index()
        {
            // Logging Information
            string userName = "Sorin";
            string lastName = "Neagu";
            _logger.LogInformation("Hello Logger! {userName} {lastName}", userName, lastName);
            // Logging Error (Exception)
            try
            {
                throw new Exception("By purpose");
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex, "Exception message");
            }
            // Logging Critical
            _logger.LogCritical("This is Critical logging!!!");


            // Memory Cache start
            var time = DateTime.Now.ToLocalTime().ToString();
            _memoryCache.Set("Time", time,
                new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = (DateTime.Now.AddMinutes(1) - DateTime.Now)
                });
            
            return View();
        }

        public ActionResult About()
        {
            string time = null;
            ViewBag.Message = _memoryCache.TryGetValue("Time", out time) ? time : DateTime.Now.ToLocalTime().ToString();

            return View();
        }

        public ActionResult Contact()
        {
            // The recommended way
            ViewBag.Message = _configuration["key1"] + " " +
                _configuration["username"] + " " +
                string.Join<int>(", ", _otherSettings.Numbers);

            //// Service locator
            //var config = DependencyResolver.Current.GetService(typeof(IConfigurationRoot)) as IConfigurationRoot;
            //ViewBag.Message = config["username"];

            //// Dynamic Bind
            //var otherSettings = new OtherSettings();
            //Configuration.GetSection("OtherSettings").Bind(otherSettings);
            //ViewBag.Message = string.Join<string>(", ", otherSettings.Strings);

            //// Using GetValue (strong typet)
            //ViewBag.Message = Configuration.GetValue<string>("username");

            return View();
        }
    }
    
}