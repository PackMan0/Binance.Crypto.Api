using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class BinanceCryptoDbContext : DbContext
    {

        public BinanceCryptoDbContext(DbContextOptions<BinanceCryptoDbContext> options) : base(options)
        {
        }

        public DbSet<PriceModel> Prices { get; set; }
    }
}
