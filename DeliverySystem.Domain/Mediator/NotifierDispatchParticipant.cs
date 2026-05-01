namespace DeliverySystem.Domain.Mediator;

public sealed class NotifierDispatchParticipant : BaseDispatchParticipant
{
    private readonly List<string> _emitted = new();

    public override string Id => "Notifier";
    public IReadOnlyList<string> Emitted => _emitted.AsReadOnly();

    protected override void OnMessage(DispatchMessage message)
    {
        switch (message.Topic)
        {
            case "courier.assigned":
                _emitted.Add($"Notify customer: courier {message.Payload} is on the way.");
                break;
            case "delivery.completed":
                _emitted.Add($"Notify customer: delivery completed {message.Payload}.");
                break;
            case "order.close":
                _emitted.Add("Notify customer: order closed, thanks!");
                break;
        }
    }
}
