namespace DeliverySystem.Domain.Mediator;

public sealed class CourierDispatchParticipant : BaseDispatchParticipant
{
    public override string Id { get; }
    public string Name { get; }

    public CourierDispatchParticipant(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Courier name is required.", nameof(name));
        Name = name;
        Id = $"Courier:{name}";
    }

    protected override void OnMessage(DispatchMessage message)
    {
        if (message.Topic == "order.ready")
        {
            Send("courier.assigned", Name);
        }
        else if (message.Topic == "delivery.start")
        {
            Send("delivery.completed", $"by {Name}");
        }
    }
}
