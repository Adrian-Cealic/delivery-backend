using DeliverySystem.Infrastructure.Payments.Adapters;
using DeliverySystem.Infrastructure.Payments.ExternalApis;
using DeliverySystem.Interfaces.Payments;

namespace DeliverySystem.Infrastructure.Payments;

public sealed class PaymentGatewayProvider : IPaymentGatewayProvider
{
    private readonly PayPalPaymentAdapter _payPalAdapter;
    private readonly StripePaymentAdapter _stripeAdapter;
    private readonly GooglePayPaymentAdapter _googlePayAdapter;

    public PaymentGatewayProvider(
        PayPalPaymentAdapter payPalAdapter,
        StripePaymentAdapter stripeAdapter,
        GooglePayPaymentAdapter googlePayAdapter)
    {
        _payPalAdapter = payPalAdapter ?? throw new ArgumentNullException(nameof(payPalAdapter));
        _stripeAdapter = stripeAdapter ?? throw new ArgumentNullException(nameof(stripeAdapter));
        _googlePayAdapter = googlePayAdapter ?? throw new ArgumentNullException(nameof(googlePayAdapter));
    }

    public IPaymentGateway GetGateway(PaymentGatewayType type)
    {
        return type switch
        {
            PaymentGatewayType.PayPal => _payPalAdapter,
            PaymentGatewayType.Stripe => _stripeAdapter,
            PaymentGatewayType.GooglePay => _googlePayAdapter,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown payment gateway type")
        };
    }
}
