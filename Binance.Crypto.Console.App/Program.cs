using Domain.Interfaces;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Binance.Crypto.Domain.Interfaces;
using Binance.Crypto.Domain.Models;
using Binance.Crypto.Domain.Services;
using Data.Context;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var runHostTask = host.RunAsync();

        using var scope = host.Services.CreateScope();
        var priceService = scope.ServiceProvider.GetRequiredService<IPriceService>();

        while (true)
        {
            Console.WriteLine("Enter command:");
            Console.WriteLine("1. 24h <symbol>");
            Console.WriteLine("2. sma <symbol> <n> <p> [<s>]");

            var input = Console.ReadLine();

            if (input.IsNullOrEmpty())
            {
                Console.WriteLine("Input cannot be empty!");
                continue;
            }

            var inputArgs = input.Split(' ');

            if (inputArgs.Length < 2)
            {
                Console.WriteLine("Invalid input!");
                continue;
            }

            var command = inputArgs[0];
            var symbol = inputArgs[1];

            if (command == "24h" && inputArgs.Length == 2)
            {
                var request = new SymbolRequestModel() { Symbol = symbol };

                if (ValidateRequest(request) == true)
                {
                    var avgPrice = await priceService.Get24hAveragePriceAsync(request);
                    Console.WriteLine($"24h average price: {avgPrice}");
                }
            }
            else if (command == "sma" && (inputArgs.Length == 4 || inputArgs.Length == 5))
            {
                var request = new SimpleMovingAverageRequestModel() { Symbol = symbol };

                int.TryParse(inputArgs[2], out var dataPointsAmount);
                request.DataPointsAmount = dataPointsAmount;
                request.TimePeriod = inputArgs[3];

                if (inputArgs.Length == 5)
                {
                    DateTime.TryParse(inputArgs[4], out var startDate);
                    request.StartDate = startDate;
                }

                if (ValidateRequest(request) == true)
                {
                    var avgPrice = await priceService.Get24hAveragePriceAsync(request);
                    Console.WriteLine($"24h average price: {avgPrice}");
                }
            }
            else
            {
                Console.WriteLine("Invalid command!");
            }
        }

        await runHostTask;
    }

    private static bool ValidateRequest(SymbolRequestModel request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(request, context, results, true);

        if (valid == false)
        {
            foreach (var result in results)
            {
                Console.WriteLine($"{result.ErrorMessage} Invalid argument: {string.Join(", ", result.MemberNames)}");
            }
        }

        return valid;
    }
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureServices((context, services) =>
                   {
                       services.AddDbContext<BinanceCryptoDbContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
                       services.AddScoped<IBinanceClient, BinanceClient>();
                       services.AddScoped<IPriceService, PriceService>();
                   });
    }
}
