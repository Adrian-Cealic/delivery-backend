using DeliverySystem.Interfaces.Payments;
using DeliverySystem.Infrastructure.Payments.ExternalApis;

namespace DeliverySystem.Infrastructure.Payments.Adapters;

public sealed class StripePaymentAdapter : IPaymentGateway
{
    private readonly StripeApi _stripeApi;
    private readonly string _defaultKey;

    public StripePaymentAdapter(StripeApi stripeApi, string defaultKey = "sk_test")
    {
        _stripeApi = stripeApi ?? throw new ArgumentNullException(nameof(stripeApi));
        _defaultKey = defaultKey ?? throw new ArgumentNullException(nameof(defaultKey));
    }

    public PaymentResult ProcessPayment(decimal amount, string currency, string referenceId)
    {
        var amountInCents = (long)Math.Round(amount * 100);
        var response = _stripeApi.Charge(_defaultKey, amountInCents, referenceId);
        return new PaymentResult(response.Success, response.TransactionId, response.ErrorMessage);
    }
}
