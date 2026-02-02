using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Entities;

public class CarCourier : Courier
{
    private const decimal CarMaxWeightKg = 50.0m;
    private const decimal MinutesPerKm = 1.5m;

    public string LicensePlate { get; private set; }

    public override VehicleType VehicleType => VehicleType.Car;
    public override decimal MaxWeight => CarMaxWeightKg;

    public CarCourier(string name, string phone, string licensePlate) : base(name, phone)
    {
        SetLicensePlate(licensePlate);
    }

    public CarCourier(Guid id, string name, string phone, string licensePlate) : base(id, name, phone)
    {
        SetLicensePlate(licensePlate);
    }

    public void SetLicensePlate(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new ArgumentException("License plate cannot be empty.", nameof(licensePlate));

        LicensePlate = licensePlate;
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
        return $"CarCourier: {Name}, Plate: {LicensePlate}, Max Weight: {MaxWeight}kg, Available: {IsAvailable}";
    }
}
