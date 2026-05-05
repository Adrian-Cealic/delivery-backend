namespace DeliverySystem.Domain.Strategies;

public sealed class EconomyDeliveryCostStrategy : IDeliveryCostStrategy
{
    private const decimal BaseFee = 2.50m;
    private const decimal PerKm = 0.45m;
    private const decimal PerKg = 0.30m;
    private const decimal Discount = 0.85m;

    public string StrategyName => "Economy";

    public decimal CalculateCost(decimal distanceKm, decimal weightKg)
    {
        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));
        if (weightKg < 0)
            throw new ArgumentException("Weight cannot be negative.", nameof(weightKg));

        var raw = BaseFee + (distanceKm * PerKm) + (weightKg * PerKg);
        return Math.Round(raw * Discount, 2);
    }

    public TimeSpan EstimatedDeliveryTime(decimal distanceKm)
    {
        var minutes = 60 + (double)distanceKm * 7;
        return TimeSpan.FromMinutes(minutes);
    }
}
