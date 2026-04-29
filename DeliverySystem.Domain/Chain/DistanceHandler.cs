namespace DeliverySystem.Domain.Chain;

public sealed class DistanceHandler : OrderValidationHandler
{
    private readonly decimal _maxDistanceKm;

    public DistanceHandler(decimal maxDistanceKm)
    {
        if (maxDistanceKm <= 0)
            throw new ArgumentException("Max distance must be positive.", nameof(maxDistanceKm));
        _maxDistanceKm = maxDistanceKm;
    }

    protected override string HandlerName => "DistanceCheck";

    protected override void Process(OrderValidationContext context, OrderValidationResult result)
    {
        if (context.DistanceKm < 0)
        {
            result.Fail(HandlerName, "Distance cannot be negative.");
            return;
        }

        if (context.DistanceKm > _maxDistanceKm)
        {
            result.Fail(HandlerName, $"Distance {context.DistanceKm:F1}km exceeds delivery range {_maxDistanceKm:F1}km.");
            return;
        }

        result.Pass(HandlerName, $"Distance {context.DistanceKm:F1}km within service area.");
    }
}
