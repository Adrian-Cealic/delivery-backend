namespace DeliverySystem.Domain.Strategies;

public sealed class StandardDeliveryCostStrategy : IDeliveryCostStrategy
{
    private const decimal BaseFee = 5.00m;
    private const decimal PerKm = 0.80m;
    private const decimal PerKg = 0.50m;

    public string StrategyName => "Standard";

    public decimal CalculateCost(decimal distanceKm, decimal weightKg)
    {
        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));
        if (weightKg < 0)
            throw new ArgumentException("Weight cannot be negative.", nameof(weightKg));

        return BaseFee + (distanceKm * PerKm) + (weightKg * PerKg);
    }

    public TimeSpan EstimatedDeliveryTime(decimal distanceKm)
    {
        var minutes = 30 + (double)distanceKm * 4;
        return TimeSpan.FromMinutes(minutes);
    }
}
