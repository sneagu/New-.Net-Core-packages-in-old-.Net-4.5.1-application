using System;
using System.Web.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {

        private IConfigurationRoot Configuration { get; set; }

        public readonly OtherSettings OtherSettings;

        public IMemoryCache MemoryCache { get; set; }

        public HomeController(IConfigurationRoot configuration, IOptions<OtherSettings> otherSettings, IMemoryCache memoryCache)
        {
            Configuration = configuration;
            OtherSettings = otherSettings.Value;
            MemoryCache = memoryCache;
        }
        public ActionResult Index()
        {
            var time = DateTime.Now.ToLocalTime().ToString();
            MemoryCache.Set("Time", time,
                new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = (DateTime.Now.AddMinutes(1) - DateTime.Now)
                });
            
            return View();
        }

        public ActionResult About()
        {
            string time = null;
            ViewBag.Message = MemoryCache.TryGetValue("Time", out time) ? time : DateTime.Now.ToLocalTime().ToString();

            return View();
        }

        public ActionResult Contact()
        {
            // The recommended way
            ViewBag.Message = Configuration["key1"] + " " +
                Configuration["username"] + " " +
                string.Join<int>(", ", OtherSettings.Numbers);

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