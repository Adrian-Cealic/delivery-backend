using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Factories;

public class BikeCourierFactory : CourierFactory
{
    public override Courier CreateCourier(CourierCreationParams parameters)
    {
        return new BikeCourier(parameters.Name, parameters.Phone);
    }
}
