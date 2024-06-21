using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
	public class PriceModel
	{
        [Key]
        public int Id { get; set; }
        public string Symbol { get; set; }
		public decimal Price { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
