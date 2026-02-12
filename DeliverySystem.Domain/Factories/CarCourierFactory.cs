using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Factories;

public class CarCourierFactory : CourierFactory
{
    public override Courier CreateCourier(CourierCreationParams parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.LicensePlate))
            throw new ArgumentException("License plate is required for car couriers.");

        return new CarCourier(parameters.Name, parameters.Phone, parameters.LicensePlate);
    }
}
