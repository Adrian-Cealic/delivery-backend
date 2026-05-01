namespace DeliverySystem.Domain.Mediator;

public sealed class OrderDispatchParticipant : BaseDispatchParticipant
{
    public override string Id { get; }

    public OrderDispatchParticipant(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("Order id is required.", nameof(orderId));
        Id = $"Order:{orderId}";
    }

    public void RequestDispatch() => Send("order.ready", $"Order {Id} ready for courier assignment.");

    protected override void OnMessage(DispatchMessage message)
    {
        switch (message.Topic)
        {
            case "courier.assigned":
                Send("order.acknowledge", $"{Id} acknowledged courier {message.Payload}.");
                break;
            case "delivery.completed":
                Send("order.close", $"{Id} closed.");
                break;
        }
    }
}
