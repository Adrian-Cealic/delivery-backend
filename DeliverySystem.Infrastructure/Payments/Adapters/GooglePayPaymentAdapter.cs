using DeliverySystem.Interfaces.Payments;
using DeliverySystem.Infrastructure.Payments.ExternalApis;

namespace DeliverySystem.Infrastructure.Payments.Adapters;

public sealed class GooglePayPaymentAdapter : IPaymentGateway
{
    private readonly GooglePayApi _googlePayApi;
    private readonly string _defaultToken;

    public GooglePayPaymentAdapter(GooglePayApi googlePayApi, string defaultToken = "gpay_wallet")
    {
        _googlePayApi = googlePayApi ?? throw new ArgumentNullException(nameof(googlePayApi));
        _defaultToken = defaultToken ?? throw new ArgumentNullException(nameof(defaultToken));
    }

    public PaymentResult ProcessPayment(decimal amount, string currency, string referenceId)
    {
        var request = new GooglePayPaymentRequest(amount, currency, referenceId);
        var response = _googlePayApi.ProcessPayment(_defaultToken, request);
        return new PaymentResult(response.Success, response.TransactionId, response.ErrorMessage);
    }
}
