namespace DeliverySystem.Domain.Chain;

public sealed class CountryRestrictionHandler : OrderValidationHandler
{
    private readonly HashSet<string> _allowedCountries;

    public CountryRestrictionHandler(IEnumerable<string> allowedCountries)
    {
        _allowedCountries = new HashSet<string>(
            allowedCountries ?? throw new ArgumentNullException(nameof(allowedCountries)),
            StringComparer.OrdinalIgnoreCase);

        if (_allowedCountries.Count == 0)
            throw new ArgumentException("At least one country must be allowed.", nameof(allowedCountries));
    }

    protected override string HandlerName => "CountryCheck";

    protected override void Process(OrderValidationContext context, OrderValidationResult result)
    {
        if (!_allowedCountries.Contains(context.CustomerCountry))
        {
            result.Fail(HandlerName, $"Country '{context.CustomerCountry}' not in service area.");
            return;
        }

        result.Pass(HandlerName, $"Country '{context.CustomerCountry}' allowed.");
    }
}
