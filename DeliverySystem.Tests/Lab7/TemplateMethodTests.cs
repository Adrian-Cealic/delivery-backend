using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Templates;

namespace DeliverySystem.Tests.Lab7;

public class TemplateMethodTests
{
    private static Order BuildOrder()
    {
        var order = new Order(Guid.NewGuid());
        order.AddItem(new OrderItem("Pizza", 2, 15m, 0.5m));
        order.AddItem(new OrderItem("Cola", 1, 5m, 0.3m));
        return order;
    }

    [Fact]
    public void OrderReceipt_ContainsHeaderLinesSummary()
    {
        var receipt = new OrderReceiptGenerator(BuildOrder(), "Alice").Generate();

        Assert.Contains("ORDER RECEIPT", receipt);
        Assert.Contains("Customer: Alice", receipt);
        Assert.Contains("Pizza", receipt);
        Assert.Contains("TOTAL:", receipt);
    }

    [Fact]
    public void OrderReceipt_IncludesMetadataSection()
    {
        var receipt = new OrderReceiptGenerator(BuildOrder(), "Alice").Generate();

        Assert.Contains("Created at:", receipt);
    }

    [Fact]
    public void DeliveryReceipt_OmitsMetadata()
    {
        var delivery = new Delivery(Guid.NewGuid(), Guid.NewGuid(), 5m);
        var receipt = new DeliveryReceiptGenerator(delivery, "Alex").Generate();

        Assert.DoesNotContain("Created at:", receipt);
        Assert.Contains("Thank you", receipt);
    }

    [Fact]
    public void RefundReceipt_ComputesRemaining()
    {
        var order = BuildOrder();
        // total = 35
        var receipt = new RefundReceiptGenerator(order, refundedAmount: 10m, reason: "damaged box").Generate();

        Assert.Contains("Refunded:", receipt);
        Assert.Contains("damaged box", receipt);
    }

    [Fact]
    public void RefundReceipt_NegativeAmount_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new RefundReceiptGenerator(BuildOrder(), -1m, "x"));
    }

    [Fact]
    public void TemplateAlgorithm_IsStable_AcrossSubclasses()
    {
        var order = BuildOrder();
        var orderReceipt = new OrderReceiptGenerator(order, "X").Generate();
        var refundReceipt = new RefundReceiptGenerator(order, 5m, "x").Generate();

        Assert.EndsWith("end of receipt --" + Environment.NewLine, orderReceipt);
        Assert.EndsWith("end of receipt --" + Environment.NewLine, refundReceipt);
    }

    [Fact]
    public void DeliveryReceipt_OverridesFooter()
    {
        var delivery = new Delivery(Guid.NewGuid(), Guid.NewGuid(), 5m);
        var receipt = new DeliveryReceiptGenerator(delivery, "Alex").Generate();

        Assert.EndsWith("end of delivery receipt --" + Environment.NewLine, receipt);
    }
}
