using DeliverySystem.Interfaces.Payments;

namespace DeliverySystem.Tests.Lab4.TestDoubles;

public sealed class FailingPaymentGateway : IPaymentGateway
{
    public PaymentResult ProcessPayment(decimal amount, string currency, string referenceId) =>
        new PaymentResult(false, null, "Simulated payment failure");
}
