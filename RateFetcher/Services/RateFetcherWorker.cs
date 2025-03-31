using RateFetcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RateFetcher.Services
{
    public class RateFetcherWorker : BackgroundService
    {
        private readonly ILogger<RateFetcherWorker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public RateFetcherWorker(ILogger<RateFetcherWorker> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string ilsUrl = "https://api.fxratesapi.com/latest?currencies=USD,EUR,GBP&base=ILS";
                    string euroUrl = "https://api.fxratesapi.com/latest?currencies=USD,GBP&base=EUR";

                    //var client = _httpClientFactory.CreateClient();
                    //var responseIls = await client.GetStringAsync(ilsUrl);
                    ////var responseEuro = await client.GetAsync(euroUrl);
                    string responseJson = @"{
                    ""success"": true,
                    ""terms"": ""https://fxratesapi.com/legal/terms-conditions"",
                    ""privacy"": ""https://fxratesapi.com/legal/privacy-policy"",
                    ""timestamp"": 1743446100,
                    ""date"": ""2025-03-31T18:35:00.000Z"",
                    ""base"": ""ILS"",
                    ""rates"": {
                        ""EUR"": 0.248083695,
                        ""GBP"": 0.207849845,
                        ""USD"": 0.268297159
                     }
                      }";
                    var exchangeRateResponse = JsonSerializer.Deserialize<FxApiResponse>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (exchangeRateResponse == null || !exchangeRateResponse.Success || exchangeRateResponse.Rates.Count == 0)
                    {
                        _logger.LogWarning("Invalid or empty exchange rate data.");
                        continue;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        
        }
    }
}
