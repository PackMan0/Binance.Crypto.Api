using Binance.Crypto.Domain.Interfaces;
using Binance.Crypto.Domain.Models;
using Data.Context;
using Data.Models;
using Domain.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Binance.Crypto.Domain.Tests.Services;

[TestFixture]
[TestOf(typeof(PriceService))]
public class PriceServiceTest
{
    [Test]
    public async Task When_DdIsEmpty_Get24hAveragePriceAsync_Should_ReturnZero()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<BinanceCryptoDbContext>()
                      .UseInMemoryDatabase(databaseName: "BinanceCryptoDB_Get24hAveragePriceAsyncIsCalled_Zero_Test")
                      .Options;

        using (var dbContextMoq = new BinanceCryptoDbContext(options))
        {
            var binanceClientMoq = new Mock<IBinanceClient>();
            var priceService = new PriceService(dbContextMoq, binanceClientMoq.Object);
            var symbolRequestModel = new SymbolRequestModel() { Symbol = "test-symbol" };

            //Act
            var result = await priceService.Get24hAveragePriceAsync(symbolRequestModel);

            //Assert
            result.Should().Be(0);
        }
    }

    [Test]
    public async Task When_Get24hAveragePriceAsyncIsCalled_Should_ReturnCorrectData()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<BinanceCryptoDbContext>()
                      .UseInMemoryDatabase(databaseName: "BinanceCryptoDB_Get24hAveragePriceAsyncIsCalled_Test")
                      .Options;

        using (var dbContextMoq = new BinanceCryptoDbContext(options))
        {
            var binanceClientMoq = new Mock<IBinanceClient>();
            var symbol = "test-symbol";
            var symbolRequestModel = new SymbolRequestModel() { Symbol = symbol };
            var prices = new List<PriceModel>();
            prices.Add(new PriceModel()
            {
                Id = 1,
                Price = 1,
                Symbol = symbol,
                Timestamp = DateTime.UtcNow.AddDays(-2)
            });
            prices.Add(new PriceModel()
            {
                Id = 2,
                Price = 2,
                Symbol = symbol + "2",
                Timestamp = DateTime.UtcNow.AddHours(-1)
            });
            prices.Add(new PriceModel()
            {
                Id = 3,
                Price = 3,
                Symbol = symbol,
                Timestamp = DateTime.UtcNow.AddHours(-2)
            });
            prices.Add(new PriceModel()
            {
                Id = 4,
                Price = 4,
                Symbol = symbol,
                Timestamp = DateTime.UtcNow.AddHours(-3)
            });

            dbContextMoq.Prices.AddRange(prices);
            dbContextMoq.SaveChanges();
            var priceService = new PriceService(dbContextMoq, binanceClientMoq.Object);

            //Act
            var result = await priceService.Get24hAveragePriceAsync(symbolRequestModel);

            //Assert
            result.Should().Be(prices.Where(p => p.Symbol == symbol && p.Timestamp >= DateTime.UtcNow.AddHours(-24)).Average(p => p.Price));
        }
    }

    [Test]
    public async Task When_GetSimpleMovingAverageAsyncIsCalled_Should_ReturnCorrectData()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<BinanceCryptoDbContext>()
                      .UseInMemoryDatabase(databaseName: "BinanceCryptoDB_GetSimpleMovingAverageAsync_Test")
                      .Options;
        using (var dbContextMoq = new BinanceCryptoDbContext(options))
        {
            var binanceClientMoq = new Mock<IBinanceClient>();
            var symbol = "test-symbol";
            var smaRequestModel = new SimpleMovingAverageRequestModel()
            {
                Symbol = symbol,
                DataPointsAmount = 3,
                StartDate = DateTime.UtcNow.AddDays(-3),
                TimePeriod = "1w"
            };
            var prices = new List<PriceModel>();
            prices.Add(new PriceModel()
            {
                Id = 12,
                Price = 1,
                Symbol = symbol,
                Timestamp = DateTime.UtcNow.AddDays(-2)
            });
            prices.Add(new PriceModel()
            {
                Id = 22,
                Price = 2,
                Symbol = symbol + "2",
                Timestamp = DateTime.UtcNow.AddHours(-1)
            });
            prices.Add(new PriceModel()
            {
                Id = 32,
                Price = 3,
                Symbol = symbol,
                Timestamp = DateTime.UtcNow.AddHours(-2)
            });
            prices.Add(new PriceModel()
            {
                Id = 42,
                Price = 4,
                Symbol = symbol,
                Timestamp = DateTime.UtcNow.AddHours(-3)
            });
            binanceClientMoq.Setup(c => c.GetKlinesAsync(It.IsAny<SimpleMovingAverageRequestModel>())).ReturnsAsync(prices);
            var priceService = new PriceService(dbContextMoq, binanceClientMoq.Object);

            //Act
            var result = await priceService.GetSimpleMovingAverageAsync(smaRequestModel);

            //Assert
            dbContextMoq.Prices.Count().Should().Be(prices.Count);
            result.Should().Be(prices.Average(p => p.Price));
        }
    }
}