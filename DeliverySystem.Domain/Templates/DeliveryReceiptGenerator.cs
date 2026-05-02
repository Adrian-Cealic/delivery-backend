using System.Text;
using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Templates;

public sealed class DeliveryReceiptGenerator : ReceiptGenerator
{
    private readonly Delivery _delivery;
    private readonly string _courierName;

    public DeliveryReceiptGenerator(Delivery delivery, string courierName)
    {
        _delivery = delivery ?? throw new ArgumentNullException(nameof(delivery));
        _courierName = courierName ?? throw new ArgumentNullException(nameof(courierName));
    }

    protected override void WriteHeader(StringBuilder sb)
    {
        sb.AppendLine("DELIVERY RECEIPT");
        sb.AppendLine($"Delivery: {_delivery.Id}");
        sb.AppendLine($"Order:    {_delivery.OrderId}");
    }

    protected override void WriteLines(StringBuilder sb)
    {
        sb.AppendLine($"Courier:        {_courierName}");
        sb.AppendLine($"Distance:       {_delivery.DistanceKm:F1} km");
        if (_delivery.PickedUpAt.HasValue)
            sb.AppendLine($"Picked up at:   {_delivery.PickedUpAt:HH:mm}");
        if (_delivery.DeliveredAt.HasValue)
            sb.AppendLine($"Delivered at:   {_delivery.DeliveredAt:HH:mm}");
    }

    protected override void WriteSummary(StringBuilder sb)
    {
        sb.AppendLine($"Final status: {_delivery.Status}");
    }

    protected override void WriteFooter(StringBuilder sb)
    {
        sb.AppendLine("Thank you for using our courier service.");
        sb.AppendLine("-- end of delivery receipt --");
    }
}
