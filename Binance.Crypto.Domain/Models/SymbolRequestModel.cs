using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Binance.Crypto.Domain.Models
{
    public class SymbolRequestModel
    {
        [Required(ErrorMessage = "symbol is required")]
        [FromRoute(Name = "symbol")]
        [RegularExpression("BTCUSDT|ADAUSDT|ETHUSDT", ErrorMessage = "Invalid Status")]
        public string Symbol { get; set; }
    }
}
