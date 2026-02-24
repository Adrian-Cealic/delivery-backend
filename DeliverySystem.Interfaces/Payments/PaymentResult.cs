namespace DeliverySystem.Interfaces.Payments;

public record PaymentResult(bool Success, string? TransactionId, string? ErrorMessage);
