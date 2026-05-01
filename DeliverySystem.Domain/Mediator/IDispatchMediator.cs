namespace DeliverySystem.Domain.Mediator;

public interface IDispatchMediator
{
    void Send(IDispatchParticipant sender, DispatchMessage message);
    void Register(IDispatchParticipant participant);
}

public interface IDispatchParticipant
{
    string Id { get; }
    void Receive(DispatchMessage message);
    void SetMediator(IDispatchMediator mediator);
}

public sealed record DispatchMessage(string Topic, string Payload, string? TargetId = null);
