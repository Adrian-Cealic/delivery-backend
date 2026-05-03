using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Visitors;

public sealed class CapacityReportVisitor : ICourierVisitor<string>
{
    public string Visit(BikeCourier courier)
        => $"{courier.Name} (Bike): {courier.MaxWeight}kg max — short-range courier";

    public string Visit(CarCourier courier)
        => $"{courier.Name} (Car, plate {courier.LicensePlate}): {courier.MaxWeight}kg max — bulk loads";

    public string Visit(DroneCourier courier)
        => $"{courier.Name} (Drone): {courier.MaxWeight}kg max within {courier.MaxFlightRangeKm}km";
}
