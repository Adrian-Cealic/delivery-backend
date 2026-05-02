using System.Text;
using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Templates;

public sealed class RefundReceiptGenerator : ReceiptGenerator
{
    private readonly Order _order;
    private readonly decimal _refundedAmount;
    private readonly string _reason;

    public RefundReceiptGenerator(Order order, decimal refundedAmount, string reason)
    {
        if (refundedAmount <= 0)
            throw new ArgumentException("Refund amount must be positive.", nameof(refundedAmount));

        _order = order ?? throw new ArgumentNullException(nameof(order));
        _refundedAmount = refundedAmount;
        _reason = reason ?? "n/a";
    }

    protected override void WriteHeader(StringBuilder sb)
    {
        sb.AppendLine("REFUND RECEIPT");
        sb.AppendLine($"Order: {_order.Id}");
        sb.AppendLine($"Issued: {DateTime.UtcNow:yyyy-MM-dd HH:mm}Z");
    }

    protected override void WriteLines(StringBuilder sb)
    {
        sb.AppendLine($"Original total: {_order.GetTotalPrice():C}");
        sb.AppendLine($"Refunded:       {_refundedAmount:C}");
        sb.AppendLine($"Reason:         {_reason}");
    }

    protected override void WriteSummary(StringBuilder sb)
    {
        var remaining = _order.GetTotalPrice() - _refundedAmount;
        sb.AppendLine($"Remaining due to customer: {Math.Max(remaining, 0m):C}");
    }
}
