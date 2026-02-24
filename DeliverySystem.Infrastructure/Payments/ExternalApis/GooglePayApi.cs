namespace DeliverySystem.Infrastructure.Payments.ExternalApis;

public sealed class GooglePayApi
{
    public GooglePayResponse ProcessPayment(string walletToken, GooglePayPaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(walletToken))
            return new GooglePayResponse(false, null, "Invalid wallet token");

        if (request == null)
            return new GooglePayResponse(false, null, "Payment request is required");

        if (request.Amount <= 0)
            return new GooglePayResponse(false, null, "Amount must be positive");

        if (string.IsNullOrWhiteSpace(request.ReferenceId))
            return new GooglePayResponse(false, null, "Reference is required");

        var transactionId = $"gpay_{Guid.NewGuid():N}";
        return new GooglePayResponse(true, transactionId, null);
    }
}

public record GooglePayPaymentRequest(decimal Amount, string Currency, string ReferenceId);

public record GooglePayResponse(bool Success, string? TransactionId, string? ErrorMessage);
