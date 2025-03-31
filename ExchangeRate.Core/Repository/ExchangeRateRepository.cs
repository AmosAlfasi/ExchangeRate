using ExchangeRate.Core.Repository.Interfaces;
using ExchangeRate.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRate.Core.Repository
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly string _filePath = Path.Combine(Path.GetTempPath(), "exchange_rates.json");
        public ExchangeRateRepository()
        {

        }
        public async Task<List<ExchangeRateModle>> GetAllRatesAsync()
        {
            
            try
            {
                var res = new List<ExchangeRateModle>();
                if (File.Exists(_filePath))
                {

                    var json = await File.ReadAllTextAsync(_filePath);
                    res = JsonSerializer.Deserialize<List<ExchangeRateModle>>(json) ?? new List<ExchangeRateModle>();
                }
                return res;

            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error reading exchange rates from file: {ex.Message}");
                return new List<ExchangeRateModle>();
            }
        }

        public async Task<ExchangeRateModle?> GetRateByPairAsync(string pairName)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return null;
                }

                await using var stream = File.OpenRead(_filePath);
                using var jsonDoc = await JsonDocument.ParseAsync(stream);
                var root = jsonDoc.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in root.EnumerateArray())
                    {
                        if (element.TryGetProperty(nameof(ExchangeRateModle.PairName), out var nameProp) &&
                            nameProp.GetString()?.Equals(pairName, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            return new ExchangeRateModle
                            {
                                PairName = nameProp.GetString()!,
                                Rate = element.GetProperty(nameof(ExchangeRateModle.Rate)).GetDecimal(),
                                LastUpdated = element.GetProperty(nameof(ExchangeRateModle.LastUpdated)).GetDateTime()
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading exchange rate for {pairName} from file: {ex.Message}");
                return null;
            }
        }
    }
}
