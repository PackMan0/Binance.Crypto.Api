using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Binance.Crypto.Domain.Models
{
    public class SimpleMovingAverageRequestModel : SymbolRequestModel
    {

        [Required(ErrorMessage = "n is required")]
        [FromQuery(Name = "n")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Invalid amount of data points")]
        public int DataPointsAmount { get; set; }

        [Required(ErrorMessage = "p is required")]
        [FromQuery(Name = "p")]
        [RegularExpression("1w|1d|30m|5m|1m", ErrorMessage = "Invalid time period")]
        public string TimePeriod { get; set; }

        [FromQuery(Name = "s")]
        public DateTime? StartDate { get; set; }
    }
}
