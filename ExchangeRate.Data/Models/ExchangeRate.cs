

namespace ExchangeRate.Data.Models
{
    public class ExchangeRateModle
    {
        public string PairName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
