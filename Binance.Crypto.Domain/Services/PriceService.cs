using Binance.Crypto.Domain.Interfaces;
using Binance.Crypto.Domain.Models;
using Data.Context;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public class PriceService : IPriceService
    {
        private readonly BinanceCryptoDbContext _dbContext;
        private readonly IBinanceClient _binanceClient;

        public PriceService(BinanceCryptoDbContext dbContext, IBinanceClient binanceClient)
        {
            _dbContext = dbContext;
            _binanceClient = binanceClient;
        }

        public async Task<decimal> Get24hAveragePriceAsync(SymbolRequestModel symbolRequestModel)
        {
            var prices = await _dbContext.Prices.Where(p => p.Symbol == symbolRequestModel.Symbol && p.Timestamp >= DateTime.UtcNow.AddHours(-24)).AsNoTracking().ToListAsync();

            if (prices.Any())
            {
                return prices.Average(p => p.Price);
            }

            return 0;
        }

        public async Task<decimal> GetSimpleMovingAverageAsync(SimpleMovingAverageRequestModel smaRequestModel)
        {
            var prices = await _binanceClient.GetKlinesAsync(smaRequestModel);

            _dbContext.AddRange(prices);
            await _dbContext.SaveChangesAsync();

            return prices.Average(p => p.Price);
        }
    }
}

