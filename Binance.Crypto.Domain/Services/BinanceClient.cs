using Binance.Crypto.Domain.Interfaces;
using Binance.Crypto.Domain.Models;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Data.Models;
namespace Binance.Crypto.Domain.Services
{
    public class BinanceClient : IBinanceClient
    {
        public async Task<List<PriceModel>> GetKlinesAsync(SimpleMovingAverageRequestModel smaModel)
        {
            var restClient = new BinanceRestClient();
            var tickerResult = await restClient.SpotApi.ExchangeData.GetKlinesAsync(smaModel.Symbol, 
                                                                                    ToKlineInterval(smaModel.TimePeriod), 
                                                                                    smaModel.StartDate?.Date ?? DateTime.UtcNow.Date, 
                                                                                    limit:smaModel.DataPointsAmount);
            return tickerResult.Data.Select(d => new PriceModel()
                               {
                                   Price = d.ClosePrice,
                                   Symbol = smaModel.Symbol,
                                   Timestamp = d.CloseTime
                               })
                               .ToList();
        }

        private static KlineInterval ToKlineInterval(string stringInterval)
        {
            switch (stringInterval)
            {
                case "1m":
                    return KlineInterval.OneMinute;
                case "5m":
                    return KlineInterval.FiveMinutes;
                case "30m":
                    return KlineInterval.ThirtyMinutes;
                case "1d":
                    return KlineInterval.OneDay;
                case "1w":
                    return KlineInterval.OneWeek;
                default:
                    return KlineInterval.OneDay;
            }
        }
    }
}
