using System.Text;
using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Templates;

public sealed class OrderReceiptGenerator : ReceiptGenerator
{
    private readonly Order _order;
    private readonly string _customerName;

    public OrderReceiptGenerator(Order order, string customerName)
    {
        _order = order ?? throw new ArgumentNullException(nameof(order));
        _customerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
    }

    protected override void WriteHeader(StringBuilder sb)
    {
        sb.AppendLine("ORDER RECEIPT");
        sb.AppendLine($"Customer: {_customerName}");
        sb.AppendLine($"Order: {_order.Id}");
        sb.AppendLine($"Issued: {DateTime.UtcNow:yyyy-MM-dd HH:mm}Z");
    }

    protected override void WriteLines(StringBuilder sb)
    {
        sb.AppendLine("Items:");
        foreach (var item in _order.Items)
        {
            sb.AppendLine($"  {item.Quantity}x {item.ProductName,-20} {item.UnitPrice,8:C} = {item.GetTotalPrice(),10:C}");
        }
    }

    protected override void WriteSummary(StringBuilder sb)
    {
        sb.AppendLine($"Status:   {_order.Status}");
        sb.AppendLine($"Priority: {_order.Priority}");
        sb.AppendLine($"TOTAL:    {_order.GetTotalPrice():C}");
    }

    protected override bool IncludeMetadata => true;

    protected override void WriteMetadata(StringBuilder sb)
    {
        sb.AppendLine($"Created at: {_order.CreatedAt:O}");
        if (_order.UpdatedAt.HasValue)
            sb.AppendLine($"Updated at: {_order.UpdatedAt:O}");
    }
}
