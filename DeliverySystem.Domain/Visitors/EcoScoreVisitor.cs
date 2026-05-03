using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Visitors;

/// <summary>
/// Returns an "eco score" 0-100. Higher is greener.
/// </summary>
public sealed class EcoScoreVisitor : ICourierVisitor<int>
{
    public int Visit(BikeCourier courier) => 95;
    public int Visit(CarCourier courier) => 35;
    public int Visit(DroneCourier courier) => 70;
}
