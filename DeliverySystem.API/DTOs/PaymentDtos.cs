namespace DeliverySystem.API.DTOs;

public record ProcessPaymentRequest(decimal Amount, string Currency, string Gateway, string? ReferenceId = null);

public record ProcessPaymentResponse(bool Success, string? TransactionId, string? ErrorMessage);
