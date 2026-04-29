namespace DeliverySystem.Domain.Chain;

public sealed class OrderValidationContext
{
    public Guid CustomerId { get; init; }
    public IReadOnlyList<OrderValidationLine> Lines { get; init; } = Array.Empty<OrderValidationLine>();
    public decimal DistanceKm { get; init; }
    public decimal WalletBalance { get; init; }
    public string CustomerCountry { get; init; } = "RO";
}

public sealed record OrderValidationLine(string ProductName, int Quantity, decimal UnitPrice, decimal Weight, int InStock);

public sealed class OrderValidationResult
{
    private readonly List<string> _passes = new();
    private readonly List<string> _failures = new();

    public bool IsAccepted => _failures.Count == 0;
    public IReadOnlyList<string> Passes => _passes;
    public IReadOnlyList<string> Failures => _failures;

    public void Pass(string handler, string detail) => _passes.Add($"{handler}: {detail}");
    public void Fail(string handler, string detail) => _failures.Add($"{handler}: {detail}");
}
