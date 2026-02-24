namespace DeliverySystem.Infrastructure.Payments.ExternalApis;

public sealed class StripeApi
{
    public StripeChargeResponse Charge(string stripeKey, long amountInCents, string idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(stripeKey))
            return new StripeChargeResponse(false, null, "Invalid API key");

        if (amountInCents <= 0)
            return new StripeChargeResponse(false, null, "Amount must be positive");

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return new StripeChargeResponse(false, null, "Idempotency key is required");

        var transactionId = $"ch_{Guid.NewGuid():N}";
        return new StripeChargeResponse(true, transactionId, null);
    }
}

public record StripeChargeResponse(bool Success, string? TransactionId, string? ErrorMessage);
