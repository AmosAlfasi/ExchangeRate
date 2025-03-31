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
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "exchange_rates.json");

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
                    var exchangeRates = await FetchExchangeRatesAsync();
                    if (exchangeRates.Any())
                    {
                        await SaveExchangeRatesToFileAsync(exchangeRates, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error Execute RateFetcherWorker");
                    throw;
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        
        }
        private async Task<List<ExchangeRate>> FetchExchangeRatesAsync()
        {
            //string responseJson = @"{
            //""success"": true,
            //""terms"": ""https://fxratesapi.com/legal/terms-conditions"",
            //""privacy"": ""https://fxratesapi.com/legal/privacy-policy"",
            //""timestamp"": 1743446100,
            //""date"": ""2025-03-31T18:35:00.000Z"",
            //""base"": ""ILS"",
            //""rates"": {
            //    ""EUR"": 0.248083695,
            //    ""GBP"": 0.207849845,
            //    ""USD"": 0.268297159
            //}
            //  }";
            List<ExchangeRate> res = new();
            try
            {
                _logger.LogInformation("Fetching exchange rates...");
                var urls = new Dictionary<string, string>
                {
                    { "ILS", "https://api.fxratesapi.com/latest?currencies=USD,EUR,GBP&base=ILS&places=3" },
                    { "EUR", "https://api.fxratesapi.com/latest?currencies=USD,GBP&base=EUR&places=3" }
                };
                var lastUpdated = DateTime.UtcNow;
                var client = _httpClientFactory.CreateClient();

                foreach (var (baseCurrency, url) in urls)
                {
                    try
                    {
                        var response = await client.GetStringAsync(url);
                        var exchangeRateResponse = JsonSerializer.Deserialize<FxApiResponse>(response, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (exchangeRateResponse is not null && exchangeRateResponse.Success )
                        {
                            foreach (var rate in exchangeRateResponse.Rates)
                            {
                                var pairName = $"{exchangeRateResponse.Base}/{rate.Key}";


                                if (!res.Any(e => e.PairName == pairName))
                                {
                                    res.Add(new ExchangeRate
                                    {
                                        PairName = pairName,
                                        Rate = rate.Value,
                                        LastUpdated = lastUpdated
                                    });
                                }
                                else
                                {
                                    _logger.LogWarning("Duplicate rate found for {PairName}, skipping...", pairName);
                                }
                            }
                           
                        }
                        else _logger.LogWarning("Invalid or empty exchange rate data for base {Base}.", baseCurrency);


                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to fetch exchange rates for {BaseCurrency}", baseCurrency);
                    }
                }




                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rates.");
                return res;
            }
        }
        private async Task SaveExchangeRatesToFileAsync(List<ExchangeRate> exchangeRates, CancellationToken stoppingToken)
        {
            try
            {
                var directoryPath = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                List<ExchangeRate> existingRates = new();
                if (File.Exists(_filePath))
                {
                    var existingJson = await File.ReadAllTextAsync(_filePath, stoppingToken);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        existingRates = JsonSerializer.Deserialize<List<ExchangeRate>>(existingJson) ?? new List<ExchangeRate>();
                    }
                }
                var updatedRates = existingRates
                                  .ToDictionary(rate => rate.PairName, rate => rate);

                foreach (var newRate in exchangeRates)
                {
                    updatedRates[newRate.PairName] = newRate;
                }
                //var jsonToSave = JsonSerializer.Serialize(exchangeRates, new JsonSerializerOptions { WriteIndented = true });
                var jsonToSave = JsonSerializer.Serialize(updatedRates.Values.ToList(), new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, jsonToSave, stoppingToken);

                _logger.LogInformation($"Exchange rates saved successfully at { DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving exchange rates to file.");
            }
        }
    }
}
