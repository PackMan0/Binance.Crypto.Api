using Binance.Crypto.Domain.Interfaces;
using Binance.Crypto.Domain.Services;
using Data.Context;
using Domain.Interfaces;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers().AddXmlSerializerFormatters();

        builder.Services.AddDbContext<BinanceCryptoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IBinanceClient,BinanceClient>();
        builder.Services.AddScoped<IPriceService, PriceService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
