using ExchangeRate.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.Core.Repository.Interfaces
{
    public interface  IExchangeRateRepository
    {
        Task<List<ExchangeRateModle>> GetAllRatesAsync();
        Task<ExchangeRateModle?> GetRateByPairAsync(string pairName);
    }
}
