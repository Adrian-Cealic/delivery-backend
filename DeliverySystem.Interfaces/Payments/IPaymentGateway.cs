namespace DeliverySystem.Interfaces.Payments;

public interface IPaymentGateway
{
    PaymentResult ProcessPayment(decimal amount, string currency, string referenceId);
}
