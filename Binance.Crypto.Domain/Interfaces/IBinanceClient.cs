using Binance.Crypto.Domain.Models;
using Data.Models;

namespace Binance.Crypto.Domain.Interfaces;

public interface IBinanceClient
{
    Task<List<PriceModel>> GetKlinesAsync(SimpleMovingAverageRequestModel smaModel);
}