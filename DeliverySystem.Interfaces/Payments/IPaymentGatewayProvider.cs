namespace DeliverySystem.Interfaces.Payments;

public enum PaymentGatewayType
{
    PayPal,
    Stripe,
    GooglePay
}

public interface IPaymentGatewayProvider
{
    IPaymentGateway GetGateway(PaymentGatewayType type);
}
