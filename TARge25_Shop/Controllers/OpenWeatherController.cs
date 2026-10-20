
using Microsoft.AspNetCore.Mvc;
using TARge25_Shop.ApplicationServices.Services;

namespace TARge25_Shop.Controllers
{
    public class OpenWeatherController : Controller
    {
        private readonly OpenWeatherService _openWeatherService;
        private readonly IConfiguration _configuration;

        public OpenWeatherController(
            OpenWeatherService openWeatherService,
            IConfiguration configuration)
        {
            _openWeatherService = openWeatherService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string city = "Tallinn")
        {
            ViewBag.City = city;

            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "Palun sisesta linna nimi.";
                return View();
            }

            string? apiKey = _configuration["OpenWeather:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ViewBag.Error = "OpenWeather API võti puudub.";
                return View();
            }

            try
            {
                string weather = await _openWeatherService
                    .GetWeatherAsync(city, apiKey);

                ViewBag.Weather = weather;
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Ilmaandmete laadimine ebaõnnestus. Kontrolli linna nime või API võtit.";
            }
            catch (TaskCanceledException)
            {
                ViewBag.Error = "Ilmaandmete päring võttis liiga kaua aega.";
            }

            return View();
        }
    }
}
