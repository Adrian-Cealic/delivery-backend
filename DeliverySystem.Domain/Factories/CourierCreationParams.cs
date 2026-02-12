namespace DeliverySystem.Domain.Factories;

public class CourierCreationParams
{
    public string Name { get; }
    public string Phone { get; }
    public string? LicensePlate { get; }
    public decimal? MaxFlightRangeKm { get; }

    public CourierCreationParams(string name, string phone, string? licensePlate = null, decimal? maxFlightRangeKm = null)
    {
        Name = name;
        Phone = phone;
        LicensePlate = licensePlate;
        MaxFlightRangeKm = maxFlightRangeKm;
    }
}
