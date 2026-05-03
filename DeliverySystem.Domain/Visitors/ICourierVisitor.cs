using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Visitors;

public interface ICourierVisitor<TResult>
{
    TResult Visit(BikeCourier courier);
    TResult Visit(CarCourier courier);
    TResult Visit(DroneCourier courier);
}
