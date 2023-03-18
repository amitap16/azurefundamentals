using AzureRedishCache.Data;
using AzureRedishCache.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureRedishCache.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, IDistributedCache cache)
        {
            _logger = logger;
            _db = dbContext;
            _cache = cache;
        }

        public IActionResult Index()
        {
            //_cache.Remove("categories");
            List<Category> categories = new();
            var cachedCategories = _cache.GetString("categories");
            if (!string.IsNullOrWhiteSpace(cachedCategories))
            {
                //from Cache
                categories = JsonConvert.DeserializeObject<List<Category>>(cachedCategories);
            }
            else
            {
                //from DB
                categories = _db.Category.ToList();
                DistributedCacheEntryOptions options = new();
                options.SetAbsoluteExpiration(new TimeSpan(0, 0, 30));
                _cache.SetString("categories", JsonConvert.SerializeObject(categories), options);
            }

            return View(categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}