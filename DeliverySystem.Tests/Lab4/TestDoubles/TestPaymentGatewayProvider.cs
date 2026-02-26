using DeliverySystem.Infrastructure.Payments.Adapters;
using DeliverySystem.Infrastructure.Payments.ExternalApis;
using DeliverySystem.Interfaces.Payments;

namespace DeliverySystem.Tests.Lab4.TestDoubles;

public sealed class TestPaymentGatewayProvider : IPaymentGatewayProvider
{
    private readonly bool _succeed;

    public TestPaymentGatewayProvider(bool succeed = true)
    {
        _succeed = succeed;
    }

    public IPaymentGateway GetGateway(PaymentGatewayType type) =>
        _succeed
            ? new PayPalPaymentAdapter(new PayPalApi())
            : new FailingPaymentGateway();
}
