using DeliverySystem.Interfaces.Payments;
using DeliverySystem.Infrastructure.Payments.ExternalApis;

namespace DeliverySystem.Infrastructure.Payments.Adapters;

public sealed class PayPalPaymentAdapter : IPaymentGateway
{
    private readonly PayPalApi _payPalApi;
    private readonly string _defaultToken;

    public PayPalPaymentAdapter(PayPalApi payPalApi, string defaultToken = "paypal_token")
    {
        _payPalApi = payPalApi ?? throw new ArgumentNullException(nameof(payPalApi));
        _defaultToken = defaultToken ?? throw new ArgumentNullException(nameof(defaultToken));
    }

    public PaymentResult ProcessPayment(decimal amount, string currency, string referenceId)
    {
        var response = _payPalApi.SendPaymentRequest(_defaultToken, amount, referenceId);
        return new PaymentResult(response.Success, response.TransactionId, response.ErrorMessage);
    }
}
