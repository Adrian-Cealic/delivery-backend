namespace DeliverySystem.Domain.Chain;

public sealed class PaymentLimitHandler : OrderValidationHandler
{
    protected override string HandlerName => "PaymentLimit";

    protected override void Process(OrderValidationContext context, OrderValidationResult result)
    {
        var orderTotal = context.Lines.Sum(l => l.Quantity * l.UnitPrice);

        if (context.WalletBalance < orderTotal)
        {
            result.Fail(HandlerName,
                $"Wallet balance {context.WalletBalance:F2} below order total {orderTotal:F2}.");
            return;
        }

        result.Pass(HandlerName, $"Wallet covers order total ({orderTotal:F2} of {context.WalletBalance:F2}).");
    }
}
