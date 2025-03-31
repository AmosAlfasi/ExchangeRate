using ExchangeRate.Core.Repository;
using ExchangeRate.Core.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IExchangeRateRepository exchangeRateRepository)
        {
            _logger = logger;
            _exchangeRateRepository = exchangeRateRepository;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>>Get()
        {
            var t = await _exchangeRateRepository.GetAllRatesAsync();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}