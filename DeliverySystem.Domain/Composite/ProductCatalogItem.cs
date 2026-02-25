using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Composite;

public sealed class ProductCatalogItem : IProductCatalogComponent
{
    public string Name { get; }
    public decimal UnitPrice { get; }
    public decimal Weight { get; }

    public ProductCatalogItem(string name, decimal unitPrice, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        if (weight < 0)
            throw new ArgumentException("Weight cannot be negative.", nameof(weight));

        Name = name;
        UnitPrice = unitPrice;
        Weight = weight;
    }

    public decimal GetTotalPrice() => UnitPrice;

    public decimal GetTotalWeight() => Weight;

    public IReadOnlyList<IProductCatalogComponent> GetChildren() =>
        Array.Empty<IProductCatalogComponent>();

    public IEnumerable<OrderItem> ToOrderItems(int quantity = 1)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        yield return new OrderItem(Name, quantity, UnitPrice, Weight);
    }
}
