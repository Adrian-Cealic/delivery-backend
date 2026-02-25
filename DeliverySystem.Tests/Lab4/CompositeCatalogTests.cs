using DeliverySystem.Domain.Composite;

namespace DeliverySystem.Tests.Lab4;

public class CompositeCatalogTests
{
    [Fact]
    public void ProductCatalogItem_ReturnsCorrectPrice()
    {
        var item = new ProductCatalogItem("Coffee", 3.50m, 0.15m);
        Assert.Equal(3.50m, item.GetTotalPrice());
        Assert.Equal(0.15m, item.GetTotalWeight());
        Assert.Equal("Coffee", item.Name);
    }

    [Fact]
    public void ProductCatalogItem_GetChildren_ReturnsEmpty()
    {
        var item = new ProductCatalogItem("Pizza", 18m, 0.6m);
        var children = item.GetChildren();
        Assert.Empty(children);
    }

    [Fact]
    public void ProductBundle_SumChildPrices()
    {
        var burger = new ProductCatalogItem("Burger", 12m, 0.4m);
        var fries = new ProductCatalogItem("Fries", 4m, 0.2m);
        var bundle = new ProductBundle("Combo");
        bundle.Add(burger);
        bundle.Add(fries);

        Assert.Equal(16m, bundle.GetTotalPrice());
        Assert.Equal(0.6m, bundle.GetTotalWeight());
        Assert.Equal(2, bundle.GetChildren().Count);
    }

    [Fact]
    public void ProductBundle_NestedBundles_SumRecursively()
    {
        var item1 = new ProductCatalogItem("A", 10m, 0.1m);
        var item2 = new ProductCatalogItem("B", 20m, 0.2m);
        var inner = new ProductBundle("Inner");
        inner.Add(item1);
        inner.Add(item2);

        var outer = new ProductBundle("Outer");
        outer.Add(inner);

        Assert.Equal(30m, outer.GetTotalPrice());
        Assert.Equal(0.3m, outer.GetTotalWeight());
    }

    [Fact]
    public void ProductCatalogItem_ToOrderItems_ReturnsSingleItem()
    {
        var item = new ProductCatalogItem("Coffee", 3.50m, 0.15m);
        var orderItems = item.ToOrderItems(2).ToList();

        Assert.Single(orderItems);
        Assert.Equal("Coffee", orderItems[0].ProductName);
        Assert.Equal(2, orderItems[0].Quantity);
        Assert.Equal(3.50m, orderItems[0].UnitPrice);
        Assert.Equal(0.15m, orderItems[0].Weight);
    }

    [Fact]
    public void ProductBundle_ToOrderItems_FlattensAllChildren()
    {
        var a = new ProductCatalogItem("A", 5m, 0.1m);
        var b = new ProductCatalogItem("B", 10m, 0.2m);
        var bundle = new ProductBundle("Bundle");
        bundle.Add(a);
        bundle.Add(b);

        var items = bundle.ToOrderItems(1).ToList();

        Assert.Equal(2, items.Count);
        Assert.Equal("A", items[0].ProductName);
        Assert.Equal("B", items[1].ProductName);
        Assert.Equal(15m, items.Sum(i => i.GetTotalPrice()));
    }

    [Fact]
    public void ProductCatalogItem_InvalidParams_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ProductCatalogItem("", 1m, 0.1m));
        Assert.Throws<ArgumentException>(() => new ProductCatalogItem("X", -1m, 0.1m));
        Assert.Throws<ArgumentException>(() => new ProductCatalogItem("X", 1m, -0.1m));
    }

    [Fact]
    public void ProductBundle_ToOrderItems_WithQuantity_MultipliesCorrectly()
    {
        var item = new ProductCatalogItem("Pizza", 18m, 0.6m);
        var bundle = new ProductBundle("Pack");
        bundle.Add(item);

        var orderItems = bundle.ToOrderItems(3).ToList();

        Assert.Single(orderItems);
        Assert.Equal(3, orderItems[0].Quantity);
        Assert.Equal(54m, orderItems[0].GetTotalPrice());
    }
}
