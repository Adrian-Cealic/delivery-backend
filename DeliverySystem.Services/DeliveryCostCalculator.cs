using DeliverySystem.Domain.Strategies;

namespace DeliverySystem.Services;

public sealed class DeliveryCostCalculator
{
    private IDeliveryCostStrategy _strategy;

    public DeliveryCostCalculator(IDeliveryCostStrategy initialStrategy)
    {
        _strategy = initialStrategy ?? throw new ArgumentNullException(nameof(initialStrategy));
    }

    public string CurrentStrategy => _strategy.StrategyName;

    public void SetStrategy(IDeliveryCostStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public DeliveryCostQuote Quote(decimal distanceKm, decimal weightKg)
    {
        var cost = _strategy.CalculateCost(distanceKm, weightKg);
        var eta = _strategy.EstimatedDeliveryTime(distanceKm);
        return new DeliveryCostQuote(_strategy.StrategyName, cost, eta);
    }
}

public sealed record DeliveryCostQuote(string Strategy, decimal Cost, TimeSpan Eta);
