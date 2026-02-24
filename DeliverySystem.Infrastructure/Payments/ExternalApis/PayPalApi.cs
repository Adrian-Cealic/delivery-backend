namespace DeliverySystem.Infrastructure.Payments.ExternalApis;

public sealed class PayPalApi
{
    public PayPalPaymentResponse SendPaymentRequest(string paypalToken, decimal amount, string reference)
    {
        if (string.IsNullOrWhiteSpace(paypalToken))
            return new PayPalPaymentResponse(false, null, "Invalid token");

        if (amount <= 0)
            return new PayPalPaymentResponse(false, null, "Amount must be positive");

        if (string.IsNullOrWhiteSpace(reference))
            return new PayPalPaymentResponse(false, null, "Reference is required");

        var transactionId = $"paypal_{Guid.NewGuid():N}";
        return new PayPalPaymentResponse(true, transactionId, null);
    }
}

public record PayPalPaymentResponse(bool Success, string? TransactionId, string? ErrorMessage);
