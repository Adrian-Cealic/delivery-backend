using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Entities;

public class DroneCourier : Courier
{
    private const decimal DroneMaxWeightKg = 2.0m;
    private const decimal MinutesPerKm = 2.0m;

    public decimal MaxFlightRangeKm { get; private set; }

    public override VehicleType VehicleType => VehicleType.Drone;
    public override decimal MaxWeight => DroneMaxWeightKg;

    public DroneCourier(string name, string phone, decimal maxFlightRangeKm) : base(name, phone)
    {
        SetMaxFlightRange(maxFlightRangeKm);
    }

    public DroneCourier(Guid id, string name, string phone, decimal maxFlightRangeKm) : base(id, name, phone)
    {
        SetMaxFlightRange(maxFlightRangeKm);
    }

    public void SetMaxFlightRange(decimal maxFlightRangeKm)
    {
        if (maxFlightRangeKm <= 0)
            throw new ArgumentException("Max flight range must be positive.", nameof(maxFlightRangeKm));

        MaxFlightRangeKm = maxFlightRangeKm;
    }

    public override TimeSpan CalculateDeliveryTime(decimal distanceKm)
    {
        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));

        if (distanceKm > MaxFlightRangeKm)
            throw new InvalidOperationException(
                $"Distance {distanceKm}km exceeds max flight range of {MaxFlightRangeKm}km.");

        var minutes = (double)(distanceKm * MinutesPerKm);
        return TimeSpan.FromMinutes(minutes);
    }

    public override string ToString()
    {
        return $"DroneCourier: {Name}, Max Weight: {MaxWeight}kg, Range: {MaxFlightRangeKm}km, Available: {IsAvailable}";
    }
}
