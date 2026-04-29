using DeliverySystem.Domain.Chain;
using DeliverySystem.Services.Chain;

namespace DeliverySystem.Tests.Lab7;

public class ChainOfResponsibilityTests
{
    private static OrderValidationContext ValidContext(decimal walletBalance = 100m) => new()
    {
        CustomerId = Guid.NewGuid(),
        Lines = new[]
        {
            new OrderValidationLine("Pizza", 2, 15m, 0.5m, 10),
            new OrderValidationLine("Cola", 1, 5m, 0.3m, 20)
        },
        DistanceKm = 12m,
        WalletBalance = walletBalance,
        CustomerCountry = "RO"
    };

    private static OrderValidationPipeline DefaultPipeline()
        => OrderValidationPipeline.Default(maxWeightKg: 10m, maxDistanceKm: 50m, allowedCountries: new[] { "RO", "MD" });

    [Fact]
    public void HappyPath_AcceptsOrder()
    {
        var result = DefaultPipeline().Validate(ValidContext());

        Assert.True(result.IsAccepted);
        Assert.Equal(5, result.Passes.Count);
    }

    [Fact]
    public void StockShortage_StopsChain_NoFurtherHandlersRun()
    {
        var ctx = ValidContext() with
        {
            Lines = new[] { new OrderValidationLine("Pizza", 5, 15m, 0.5m, 2) }
        };

        var result = DefaultPipeline().Validate(ctx);

        Assert.False(result.IsAccepted);
        Assert.Single(result.Failures);
        Assert.Contains("StockCheck", result.Failures[0]);
        Assert.Empty(result.Passes);
    }

    [Fact]
    public void WeightOverLimit_FailsButOtherHandlersStillRun()
    {
        var ctx = ValidContext() with
        {
            Lines = new[] { new OrderValidationLine("Brick", 30, 1m, 1m, 100) }
        };

        var result = DefaultPipeline().Validate(ctx);

        Assert.False(result.IsAccepted);
        Assert.Contains(result.Failures, f => f.StartsWith("WeightLimit"));
        Assert.Contains(result.Passes, p => p.StartsWith("StockCheck"));
    }

    [Fact]
    public void DistanceOver_FailsAtDistanceHandler()
    {
        var ctx = ValidContext() with { DistanceKm = 200m };

        var result = DefaultPipeline().Validate(ctx);

        Assert.Contains(result.Failures, f => f.StartsWith("DistanceCheck"));
    }

    [Fact]
    public void DisallowedCountry_FailsAtCountryHandler()
    {
        var ctx = ValidContext() with { CustomerCountry = "FR" };

        var result = DefaultPipeline().Validate(ctx);

        Assert.Contains(result.Failures, f => f.StartsWith("CountryCheck"));
    }

    [Fact]
    public void InsufficientWallet_FailsAtPaymentHandler()
    {
        var ctx = ValidContext(walletBalance: 5m);

        var result = DefaultPipeline().Validate(ctx);

        Assert.Contains(result.Failures, f => f.StartsWith("PaymentLimit"));
    }

    [Fact]
    public void ChainBuilder_LinksHandlersInOrder()
    {
        var stock = new StockValidationHandler();
        var weight = new WeightLimitHandler(10m);
        var nextRef = stock.SetNext(weight);

        Assert.Same(weight, nextRef);
    }

    [Fact]
    public void EmptyOrder_FailsImmediately()
    {
        var ctx = ValidContext() with { Lines = Array.Empty<OrderValidationLine>() };

        var result = DefaultPipeline().Validate(ctx);

        Assert.False(result.IsAccepted);
        Assert.Contains("StockCheck", result.Failures[0]);
    }
}
