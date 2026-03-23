using System.Text.Json;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Infrastructure.Reports;
using DeliverySystem.Services.Reports;
using DeliverySystem.Tests.Lab5.TestDoubles;

namespace DeliverySystem.Tests.Lab5;

public class BridgeTests
{
    private static List<Order> CreateTestOrders()
    {
        var customerId = Guid.NewGuid();
        var order1 = new Order(customerId);
        order1.AddItem(new OrderItem("Pizza", 2, 15.00m, 0.5m));
        var order2 = new Order(customerId);
        order2.AddItem(new OrderItem("Burger", 1, 10.00m, 0.3m));
        return new List<Order> { order1, order2 };
    }

    private static List<Delivery> CreateTestDeliveries()
    {
        return new List<Delivery>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), 5.0m),
            new(Guid.NewGuid(), Guid.NewGuid(), 12.5m)
        };
    }

    [Fact]
    public void OrderSummaryReport_ConsoleRenderer_ProducesTextOutput()
    {
        var renderer = new ConsoleReportRenderer();
        var report = new OrderSummaryReport(renderer, CreateTestOrders());

        var result = report.Generate();

        Assert.Contains("Order Summary Report", result);
        Assert.Contains("Total Orders", result);
        Assert.Contains("2", result);
    }

    [Fact]
    public void OrderSummaryReport_JsonRenderer_ProducesValidJson()
    {
        var renderer = new JsonReportRenderer();
        var report = new OrderSummaryReport(renderer, CreateTestOrders());

        var result = report.Generate();

        var parsed = JsonSerializer.Deserialize<JsonElement>(result);
        Assert.Equal("Order Summary Report", parsed.GetProperty("Title").GetString());
        Assert.True(parsed.GetProperty("Data").GetProperty("Total Orders").GetString() == "2");
    }

    [Fact]
    public void DeliveryStatusReport_ConsoleRenderer_ProducesTextOutput()
    {
        var renderer = new ConsoleReportRenderer();
        var report = new DeliveryStatusReport(renderer, CreateTestDeliveries());

        var result = report.Generate();

        Assert.Contains("Delivery Status Report", result);
        Assert.Contains("Total Deliveries", result);
        Assert.Contains("2", result);
    }

    [Fact]
    public void DeliveryStatusReport_JsonRenderer_ProducesValidJson()
    {
        var renderer = new JsonReportRenderer();
        var report = new DeliveryStatusReport(renderer, CreateTestDeliveries());

        var result = report.Generate();

        var parsed = JsonSerializer.Deserialize<JsonElement>(result);
        Assert.Equal("Delivery Status Report", parsed.GetProperty("Title").GetString());
        Assert.Equal("2", parsed.GetProperty("Data").GetProperty("Total Deliveries").GetString());
    }

    [Fact]
    public void RendererSwappable_SameReportDifferentOutput()
    {
        var orders = CreateTestOrders();
        var consoleResult = new OrderSummaryReport(new ConsoleReportRenderer(), orders).Generate();
        var jsonResult = new OrderSummaryReport(new JsonReportRenderer(), orders).Generate();

        Assert.NotEqual(consoleResult, jsonResult);
        Assert.Contains("===", consoleResult);
        Assert.Contains("{", jsonResult);
    }

    [Fact]
    public void CapturingRenderer_ReceivesCorrectData()
    {
        var renderer = new CapturingReportRenderer();
        var report = new OrderSummaryReport(renderer, CreateTestOrders());

        var result = report.Generate();

        Assert.Equal("Order Summary Report", renderer.LastTitle);
        Assert.NotNull(renderer.LastData);
        Assert.Equal("2", renderer.LastData!["Total Orders"]);
        Assert.Equal("CAPTURED:Order Summary Report", result);
    }
}
