using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using WeatherApp.Models;
using WeatherApp.Models.WeatherHistory;

namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherService _weatherService;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, WeatherService weatherService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _weatherService = weatherService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string city = "Cherkasy")
        {
            var weather = await _weatherService.GetWeeklyForecastAsync(city);
            return View(weather);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public IActionResult Chat()
        {
            return View(); 
        }
        [Authorize]
        public IActionResult Maps()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Historical() => View();

        [HttpPost]
        public async Task<IActionResult> Historical(
            string city,
            DateTime start,
            DateTime end,
            [FromForm] string[] metrics,
            string granularity
        )
        {
            ViewBag.City = city;
            ViewBag.Start = start.ToString("yyyy-MM-dd");
            ViewBag.End = end.ToString("yyyy-MM-dd");
            ViewBag.Metrics = metrics;
            ViewBag.Granularity = granularity;

            HistoricalForecast model;
            try
            {
                model = await _weatherService.GetHistoricalAsync(city, start, end, metrics);
            }
            catch
            {
                ModelState.AddModelError("", "Не вдалося отримати історичні дані.");
                return View(null);
            }

            // --- Вот здесь, после получения model, собираем словарь для ViewBag.Data ---
            var dict = new Dictionary<string, object>();
            foreach (var m in metrics)
            {
                var values = m switch
                {
                    "temperature_2m" => model.Hourly.Temperature_2m,
                    "relativehumidity_2m" => model.Hourly.Relativehumidity_2m,
                    "windspeed_10m" => model.Hourly.Windspeed_10m,
                    "precipitation" => model.Hourly.Precipitation,
                    "pressure_msl" => model.Hourly.Pressure_msl,
                    _ => null
                };
                dict[m] = values;
            }
            ViewBag.Data = dict;
            // ------------------------------------------------------------------------------

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> FutureWeather(string city, string dt)
        {
            var model = await _weatherService.GetFutureForecastAsync(city, dt);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
