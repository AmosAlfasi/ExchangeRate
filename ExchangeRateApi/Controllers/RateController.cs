using ExchangeRate.Core.Repository.Interfaces;
using ExchangeRate.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public RateController(IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExchangeRateModle>>> GetExchangeRates()
        {
            var rates = await _exchangeRateRepository.GetAllRatesAsync();
            return Ok(rates);
        }

        // Endpoint לשליפת שער חליפין עבור זוג מסויים
        [HttpGet("{pairName}")]
        public async Task<ActionResult<ExchangeRateModle>> GetExchangeRateByPair(string pairName)
        {
            string decodedPairName = Uri.UnescapeDataString(pairName);
            var rate = await _exchangeRateRepository.GetRateByPairAsync(decodedPairName);
         
            if (rate is null)
            {
                return NotFound($"Exchange rate for {pairName} not found.");
            }
            return Ok(rate);
        }
    }
}
