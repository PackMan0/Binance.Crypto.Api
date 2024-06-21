using Binance.Crypto.Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Binance.Crypto.Api.Controllers
{
	[ApiController]
	[Route("api/")]
	public class PriceController : ControllerBase
	{
		private readonly IPriceService _priceService;

		public PriceController(IPriceService priceService)
		{
			_priceService = priceService;
		}

		[HttpGet("{symbol}/24hAvgPrice")]
		public async Task<IActionResult> Get24hAvgPrice(SymbolRequestModel symbolRequestModel)
		{
            var result = await _priceService.Get24hAveragePriceAsync(symbolRequestModel);
            return Ok(result);
        }

		[HttpGet("{symbol}/SimpleMovingAverage")]
		public async Task<IActionResult> GetSimpleMovingAverage(SimpleMovingAverageRequestModel smaRequestModel)
		{
            var result = await _priceService.GetSimpleMovingAverageAsync(smaRequestModel);
            return Ok(result);
		}
	}
}
