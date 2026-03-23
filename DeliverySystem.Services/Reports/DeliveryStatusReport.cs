using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces.Reports;

namespace DeliverySystem.Services.Reports;

public sealed class DeliveryStatusReport : DeliveryReport
{
    private readonly IEnumerable<Delivery> _deliveries;

    public DeliveryStatusReport(IReportRenderer renderer, IEnumerable<Delivery> deliveries)
        : base(renderer)
    {
        _deliveries = deliveries ?? throw new ArgumentNullException(nameof(deliveries));
    }

    public override string Generate()
    {
        var deliveryList = _deliveries.ToList();
        var data = new Dictionary<string, string>
        {
            ["Total Deliveries"] = deliveryList.Count.ToString(),
            ["Pending"] = deliveryList.Count(d => d.Status == DeliveryStatus.Pending).ToString(),
            ["Assigned"] = deliveryList.Count(d => d.Status == DeliveryStatus.Assigned).ToString(),
            ["In Transit"] = deliveryList.Count(d => d.Status == DeliveryStatus.InTransit).ToString(),
            ["Delivered"] = deliveryList.Count(d => d.Status == DeliveryStatus.Delivered).ToString(),
            ["Failed"] = deliveryList.Count(d => d.Status == DeliveryStatus.Failed).ToString(),
            ["Total Distance (km)"] = deliveryList.Sum(d => d.DistanceKm).ToString("F1")
        };

        return Renderer.Render("Delivery Status Report", data);
    }
}
