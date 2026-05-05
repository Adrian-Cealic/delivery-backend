namespace DeliverySystem.Domain.Strategies;

public sealed class ExpressDeliveryCostStrategy : IDeliveryCostStrategy
{
    private const decimal BaseFee = 12.00m;
    private const decimal PerKm = 1.50m;
    private const decimal PerKg = 0.80m;
    private const decimal PriorityMultiplier = 1.35m;

    public string StrategyName => "Express";

    public decimal CalculateCost(decimal distanceKm, decimal weightKg)
    {
        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));
        if (weightKg < 0)
            throw new ArgumentException("Weight cannot be negative.", nameof(weightKg));

        var raw = BaseFee + (distanceKm * PerKm) + (weightKg * PerKg);
        return Math.Round(raw * PriorityMultiplier, 2);
    }

    public TimeSpan EstimatedDeliveryTime(decimal distanceKm)
    {
        var minutes = 15 + (double)distanceKm * 2;
        return TimeSpan.FromMinutes(minutes);
    }
}
