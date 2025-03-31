

namespace ExchangeRateModels.Models
{
    public class ExchangeRate
    {
        public string PairName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
