namespace DeliverySystem.Domain.Chain;

public sealed class StockValidationHandler : OrderValidationHandler
{
    protected override string HandlerName => "StockCheck";
    protected override bool StopOnFailure => true;

    protected override void Process(OrderValidationContext context, OrderValidationResult result)
    {
        if (context.Lines.Count == 0)
        {
            result.Fail(HandlerName, "Order has no lines.");
            return;
        }

        foreach (var line in context.Lines)
        {
            if (line.Quantity > line.InStock)
            {
                result.Fail(HandlerName, $"'{line.ProductName}' requires {line.Quantity} but only {line.InStock} in stock.");
                return;
            }
        }

        result.Pass(HandlerName, $"{context.Lines.Count} line(s) covered by stock.");
    }
}
