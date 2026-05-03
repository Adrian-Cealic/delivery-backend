using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Visitors;

/// <summary>
/// Double dispatch entry point. Adding a new courier subtype requires extending this switch
/// (and adding a Visit overload to ICourierVisitor) — explicit and compile-time safe.
/// </summary>
public static class CourierVisitorExtensions
{
    public static TResult Accept<TResult>(this Courier courier, ICourierVisitor<TResult> visitor)
    {
        if (courier == null) throw new ArgumentNullException(nameof(courier));
        if (visitor == null) throw new ArgumentNullException(nameof(visitor));

        return courier switch
        {
            BikeCourier bike => visitor.Visit(bike),
            CarCourier car => visitor.Visit(car),
            DroneCourier drone => visitor.Visit(drone),
            _ => throw new NotSupportedException($"Visitor does not handle '{courier.GetType().Name}'.")
        };
    }
}
