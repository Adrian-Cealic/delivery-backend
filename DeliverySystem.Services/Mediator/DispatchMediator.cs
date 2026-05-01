using DeliverySystem.Domain.Mediator;

namespace DeliverySystem.Services.Mediator;

/// <summary>
/// Concrete mediator that routes DispatchMessage between registered participants.
/// Participants never reference each other directly — all communication goes through here.
/// </summary>
public sealed class DispatchMediator : IDispatchMediator
{
    private readonly Dictionary<string, IDispatchParticipant> _participants = new();
    private readonly List<string> _log = new();

    public IReadOnlyList<string> Log => _log.AsReadOnly();
    public IReadOnlyCollection<string> RegisteredIds => _participants.Keys.ToArray();

    public void Register(IDispatchParticipant participant)
    {
        if (participant == null) throw new ArgumentNullException(nameof(participant));

        _participants[participant.Id] = participant;
        participant.SetMediator(this);
        _log.Add($"REGISTER {participant.Id}");
    }

    public void Send(IDispatchParticipant sender, DispatchMessage message)
    {
        _log.Add($"{sender.Id} -> {message.TargetId ?? "*"} [{message.Topic}] {message.Payload}");

        if (message.TargetId != null)
        {
            if (_participants.TryGetValue(message.TargetId, out var direct))
                direct.Receive(message);
            return;
        }

        foreach (var (id, participant) in _participants)
        {
            if (id == sender.Id) continue;
            participant.Receive(message);
        }
    }
}
