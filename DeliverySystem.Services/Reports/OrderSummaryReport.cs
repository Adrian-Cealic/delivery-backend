using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces.Reports;

namespace DeliverySystem.Services.Reports;

public sealed class OrderSummaryReport : DeliveryReport
{
    private readonly IEnumerable<Order> _orders;

    public OrderSummaryReport(IReportRenderer renderer, IEnumerable<Order> orders)
        : base(renderer)
    {
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
    }

    public override string Generate()
    {
        var orderList = _orders.ToList();
        var data = new Dictionary<string, string>
        {
            ["Total Orders"] = orderList.Count.ToString(),
            ["Total Revenue"] = orderList.Sum(o => o.GetTotalPrice()).ToString("C"),
            ["Avg Order Value"] = orderList.Count > 0
                ? (orderList.Sum(o => o.GetTotalPrice()) / orderList.Count).ToString("C")
                : "N/A",
            ["Total Items"] = orderList.Sum(o => o.Items.Count).ToString(),
            ["Created"] = orderList.Count(o => o.Status == Domain.Enums.OrderStatus.Created).ToString(),
            ["Confirmed"] = orderList.Count(o => o.Status == Domain.Enums.OrderStatus.Confirmed).ToString(),
            ["Delivered"] = orderList.Count(o => o.Status == Domain.Enums.OrderStatus.Delivered).ToString()
        };

        return Renderer.Render("Order Summary Report", data);
    }
}
