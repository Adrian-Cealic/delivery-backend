namespace DeliverySystem.API.DTOs;

// Chain of Responsibility
public record OrderValidationLineDto(string ProductName, int Quantity, decimal UnitPrice, decimal Weight, int InStock);
public record OrderValidationRequest(
    Guid CustomerId,
    IReadOnlyList<OrderValidationLineDto> Lines,
    decimal DistanceKm,
    decimal WalletBalance,
    string CustomerCountry);
public record OrderValidationResponse(bool Accepted, IReadOnlyList<string> Passes, IReadOnlyList<string> Failures);

// State machine
public record StateActionRequest(string Action, string? Reason);
public record StateSnapshotDto(string CurrentState, bool IsTerminal, string? FailureReason, IReadOnlyList<string> Trace);

// Mediator
public record MediatorParticipantDto(string Kind, string Name);
public record MediatorBroadcastDto(IReadOnlyList<string> Registered, IReadOnlyList<string> Log, IReadOnlyList<string> NotifierEmitted);

// Template Method
public record ReceiptRequest(string Kind, Guid Id, decimal? RefundAmount, string? RefundReason, string? CustomerName, string? CourierName);
public record ReceiptResponse(string Kind, string Body);

// Visitor
public record VisitorRequest(string Visitor);
public record VisitorRowDto(string Courier, string Vehicle, string Result);
public record VisitorResponse(string Visitor, IReadOnlyList<VisitorRowDto> Rows);
