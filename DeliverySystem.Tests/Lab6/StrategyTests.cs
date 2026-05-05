using DeliverySystem.Domain.Strategies;
using DeliverySystem.Services;

namespace DeliverySystem.Tests.Lab6;

public class StrategyTests
{
    [Fact]
    public void StandardStrategy_CalculatesCostWithBaseAndDistance()
    {
        var strategy = new StandardDeliveryCostStrategy();

        var cost = strategy.CalculateCost(distanceKm: 10m, weightKg: 2m);

        // 5 + 10*0.80 + 2*0.50 = 14
        Assert.Equal(14.00m, cost);
    }

    [Fact]
    public void ExpressStrategy_AppliesPriorityMultiplier()
    {
        var strategy = new ExpressDeliveryCostStrategy();

        var cost = strategy.CalculateCost(distanceKm: 10m, weightKg: 2m);

        // (12 + 10*1.50 + 2*0.80) * 1.35 = 28.6 * 1.35 = 38.61
        Assert.Equal(38.61m, cost);
    }

    [Fact]
    public void EconomyStrategy_AppliesDiscount()
    {
        var strategy = new EconomyDeliveryCostStrategy();

        var cost = strategy.CalculateCost(distanceKm: 10m, weightKg: 2m);

        // (2.5 + 10*0.45 + 2*0.30) * 0.85 = 7.6 * 0.85 = 6.46
        Assert.Equal(6.46m, cost);
    }

    [Fact]
    public void Calculator_SwitchesStrategyAtRuntime()
    {
        var calc = new DeliveryCostCalculator(new StandardDeliveryCostStrategy());
        var standardQuote = calc.Quote(distanceKm: 10m, weightKg: 2m);

        calc.SetStrategy(new ExpressDeliveryCostStrategy());
        var expressQuote = calc.Quote(distanceKm: 10m, weightKg: 2m);

        Assert.Equal("Standard", standardQuote.Strategy);
        Assert.Equal("Express", expressQuote.Strategy);
        Assert.NotEqual(standardQuote.Cost, expressQuote.Cost);
    }

    [Fact]
    public void ExpressStrategy_DeliversFasterThanStandard()
    {
        var express = new ExpressDeliveryCostStrategy();
        var standard = new StandardDeliveryCostStrategy();

        var expressEta = express.EstimatedDeliveryTime(20m);
        var standardEta = standard.EstimatedDeliveryTime(20m);

        Assert.True(expressEta < standardEta);
    }

    [Fact]
    public void EconomyStrategy_DeliversSlowerThanStandard()
    {
        var economy = new EconomyDeliveryCostStrategy();
        var standard = new StandardDeliveryCostStrategy();

        var economyEta = economy.EstimatedDeliveryTime(20m);
        var standardEta = standard.EstimatedDeliveryTime(20m);

        Assert.True(economyEta > standardEta);
    }

    [Fact]
    public void CalculateCost_NegativeDistance_Throws()
    {
        var strategy = new StandardDeliveryCostStrategy();

        Assert.Throws<ArgumentException>(() => strategy.CalculateCost(-1m, 2m));
    }

    [Fact]
    public void Calculator_NullStrategy_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DeliveryCostCalculator(null!));
    }
}
