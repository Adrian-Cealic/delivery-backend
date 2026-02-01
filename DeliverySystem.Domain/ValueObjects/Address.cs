namespace DeliverySystem.Domain.ValueObjects;

public sealed class Address
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty.", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty.", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty.", nameof(country));

        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public string GetFullAddress()
    {
        return $"{Street}, {City}, {PostalCode}, {Country}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Address other)
            return false;

        return Street == other.Street
            && City == other.City
            && PostalCode == other.PostalCode
            && Country == other.Country;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, PostalCode, Country);
    }

    public override string ToString()
    {
        return GetFullAddress();
    }
}
