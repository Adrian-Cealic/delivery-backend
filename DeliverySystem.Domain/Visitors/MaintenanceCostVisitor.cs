using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Visitors;

/// <summary>
/// Computes a monthly maintenance cost without touching the courier classes.
/// Each branch encodes domain-specific knowledge (tires for bikes, fuel for cars,
/// battery + parts for drones).
/// </summary>
public sealed class MaintenanceCostVisitor : ICourierVisitor<decimal>
{
    public decimal Visit(BikeCourier courier) => 35m;          // tires, lube, brakes

    public decimal Visit(CarCourier courier) => 220m + 0.10m * 1000m; // service + ~1000km fuel allowance

    public decimal Visit(DroneCourier courier)
        => 90m + courier.MaxFlightRangeKm * 0.50m; // battery + range-dependent parts
}
