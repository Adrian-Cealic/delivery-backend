namespace DeliverySystem.Domain.Mediator;

public abstract class BaseDispatchParticipant : IDispatchParticipant
{
    private IDispatchMediator? _mediator;
    private readonly List<string> _inbox = new();

    public abstract string Id { get; }
    public IReadOnlyList<string> Inbox => _inbox.AsReadOnly();

    public void SetMediator(IDispatchMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public void Receive(DispatchMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        _inbox.Add($"[{message.Topic}] {message.Payload}");
        OnMessage(message);
    }

    protected virtual void OnMessage(DispatchMessage message) { }

    protected void Send(string topic, string payload, string? targetId = null)
    {
        if (_mediator == null)
            throw new InvalidOperationException($"Participant '{Id}' is not registered with a mediator.");

        _mediator.Send(this, new DispatchMessage(topic, payload, targetId));
    }
}
