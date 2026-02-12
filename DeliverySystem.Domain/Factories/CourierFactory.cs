using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Factories;

public abstract class CourierFactory
{
    public abstract Courier CreateCourier(CourierCreationParams parameters);

    public Courier CreateAndValidate(CourierCreationParams parameters)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        if (string.IsNullOrWhiteSpace(parameters.Name))
            throw new ArgumentException("Courier name is required.");

        if (string.IsNullOrWhiteSpace(parameters.Phone))
            throw new ArgumentException("Courier phone is required.");

        var courier = CreateCourier(parameters);
        return courier;
    }
}
