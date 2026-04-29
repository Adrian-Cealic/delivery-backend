using DeliverySystem.Domain.Chain;

namespace DeliverySystem.Services.Chain;

/// <summary>
/// Builds and runs an order validation chain. The first handler is exposed as the entry point;
/// successive handlers are linked via SetNext to keep the chain explicit.
/// </summary>
public sealed class OrderValidationPipeline
{
    private readonly OrderValidationHandler _entry;

    public OrderValidationPipeline(OrderValidationHandler entry)
    {
        _entry = entry ?? throw new ArgumentNullException(nameof(entry));
    }

    public OrderValidationResult Validate(OrderValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var result = new OrderValidationResult();
        _entry.Handle(context, result);
        return result;
    }

    public static OrderValidationPipeline Default(decimal maxWeightKg, decimal maxDistanceKm, IEnumerable<string> allowedCountries)
    {
        var stock = new StockValidationHandler();
        var weight = new WeightLimitHandler(maxWeightKg);
        var distance = new DistanceHandler(maxDistanceKm);
        var country = new CountryRestrictionHandler(allowedCountries);
        var payment = new PaymentLimitHandler();

        stock.SetNext(weight).SetNext(distance).SetNext(country).SetNext(payment);
        return new OrderValidationPipeline(stock);
    }
}
