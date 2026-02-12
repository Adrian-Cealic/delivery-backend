using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Factories;

public static class CourierFactoryProvider
{
    private static readonly Dictionary<VehicleType, CourierFactory> Factories = new()
    {
        { VehicleType.Bicycle, new BikeCourierFactory() },
        { VehicleType.Car, new CarCourierFactory() },
        { VehicleType.Drone, new DroneCourierFactory() }
    };

    public static CourierFactory GetFactory(VehicleType vehicleType)
    {
        if (!Factories.TryGetValue(vehicleType, out var factory))
            throw new ArgumentException($"No factory registered for vehicle type: {vehicleType}");

        return factory;
    }

    public static IReadOnlyCollection<VehicleType> GetSupportedTypes()
    {
        return Factories.Keys.ToList().AsReadOnly();
    }
}
