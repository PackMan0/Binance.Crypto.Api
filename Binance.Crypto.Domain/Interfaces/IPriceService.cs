using Binance.Crypto.Domain.Models;

namespace Domain.Interfaces
{
	public interface IPriceService
	{
		Task<decimal> Get24hAveragePriceAsync(SymbolRequestModel symbolRequestModel);
		Task<decimal> GetSimpleMovingAverageAsync(SimpleMovingAverageRequestModel smaRequestModel);
	}
}
