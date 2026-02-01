namespace DeliverySystem.Domain.Entities;

public sealed class OrderItem
{
    public string ProductName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal Weight { get; }

    public OrderItem(string productName, int quantity, decimal unitPrice, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty.", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        if (weight < 0)
            throw new ArgumentException("Weight cannot be negative.", nameof(weight));

        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Weight = weight;
    }

    public decimal GetTotalPrice()
    {
        return Quantity * UnitPrice;
    }

    public decimal GetTotalWeight()
    {
        return Quantity * Weight;
    }

    public override string ToString()
    {
        return $"{ProductName} x{Quantity} @ {UnitPrice:C} (Weight: {Weight}kg)";
    }
}
