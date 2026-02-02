using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Entities;

public class BikeCourier : Courier
{
    private const decimal BikeMaxWeightKg = 5.0m;
    private const decimal MinutesPerKm = 3.0m;

    public override VehicleType VehicleType => VehicleType.Bicycle;
    public override decimal MaxWeight => BikeMaxWeightKg;

    public BikeCourier(string name, string phone) : base(name, phone)
    {
    }

    public BikeCourier(Guid id, string name, string phone) : base(id, name, phone)
    {
    }

    public override TimeSpan CalculateDeliveryTime(decimal distanceKm)
    {
        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));

        var minutes = (double)(distanceKm * MinutesPerKm);
        return TimeSpan.FromMinutes(minutes);
    }

    public override string ToString()
    {
        return $"BikeCourier: {Name}, Max Weight: {MaxWeight}kg, Available: {IsAvailable}";
    }
}
