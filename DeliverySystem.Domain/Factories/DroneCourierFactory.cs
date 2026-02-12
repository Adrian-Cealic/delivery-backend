using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Factories;

public class DroneCourierFactory : CourierFactory
{
    public override Courier CreateCourier(CourierCreationParams parameters)
    {
        if (!parameters.MaxFlightRangeKm.HasValue || parameters.MaxFlightRangeKm <= 0)
            throw new ArgumentException("Max flight range is required for drone couriers.");

        return new DroneCourier(parameters.Name, parameters.Phone, parameters.MaxFlightRangeKm.Value);
    }
}
