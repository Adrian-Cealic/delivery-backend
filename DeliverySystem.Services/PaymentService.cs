using DeliverySystem.Interfaces.Payments;

namespace DeliverySystem.Services;

public sealed class PaymentService
{
    private readonly IPaymentGatewayProvider _gatewayProvider;

    public PaymentService(IPaymentGatewayProvider gatewayProvider)
    {
        _gatewayProvider = gatewayProvider ?? throw new ArgumentNullException(nameof(gatewayProvider));
    }

    public PaymentResult ProcessPayment(decimal amount, string currency, string referenceId, PaymentGatewayType gatewayType)
    {
        var gateway = _gatewayProvider.GetGateway(gatewayType);
        return gateway.ProcessPayment(amount, currency, referenceId);
    }
}
