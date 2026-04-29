namespace DeliverySystem.Domain.Chain;

public sealed class WeightLimitHandler : OrderValidationHandler
{
    private readonly decimal _maxWeightKg;

    public WeightLimitHandler(decimal maxWeightKg)
    {
        if (maxWeightKg <= 0)
            throw new ArgumentException("Max weight must be positive.", nameof(maxWeightKg));
        _maxWeightKg = maxWeightKg;
    }

    protected override string HandlerName => "WeightLimit";

    protected override void Process(OrderValidationContext context, OrderValidationResult result)
    {
        var totalWeight = context.Lines.Sum(l => l.Quantity * l.Weight);

        if (totalWeight > _maxWeightKg)
        {
            result.Fail(HandlerName, $"Total weight {totalWeight:F2}kg exceeds limit of {_maxWeightKg:F2}kg.");
            return;
        }

        result.Pass(HandlerName, $"Total weight {totalWeight:F2}kg is within limit.");
    }
}
