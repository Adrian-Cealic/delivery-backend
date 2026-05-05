namespace DeliverySystem.Domain.Strategies;

public interface IDeliveryCostStrategy
{
    string StrategyName { get; }
    decimal CalculateCost(decimal distanceKm, decimal weightKg);
    TimeSpan EstimatedDeliveryTime(decimal distanceKm);
}
