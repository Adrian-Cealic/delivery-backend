namespace DeliverySystem.Domain.Commands;

/// <summary>
/// Composite command that executes a sequence of delivery commands as a single unit.
/// Useful for scripted dispatch flows (assign → pick up → in transit).
/// </summary>
public sealed class MacroDeliveryCommand : IDeliveryCommand
{
    private readonly IReadOnlyList<IDeliveryCommand> _commands;
    private readonly string _label;
    private int _executedCount;

    public MacroDeliveryCommand(string label, IEnumerable<IDeliveryCommand> commands)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Macro label is required.", nameof(label));
        if (commands == null) throw new ArgumentNullException(nameof(commands));

        _label = label;
        _commands = commands.ToList();
        if (_commands.Count == 0)
            throw new ArgumentException("Macro must contain at least one command.", nameof(commands));
    }

    public string Description => $"Macro [{_label}] x{_commands.Count}";

    public void Execute()
    {
        for (int i = _executedCount; i < _commands.Count; i++)
        {
            _commands[i].Execute();
            _executedCount = i + 1;
        }
    }

    public void Undo()
    {
        for (int i = _executedCount - 1; i >= 0; i--)
        {
            _commands[i].Undo();
            _executedCount = i;
        }
    }
}
