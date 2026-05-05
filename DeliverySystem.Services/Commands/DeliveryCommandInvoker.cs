using DeliverySystem.Domain.Commands;

namespace DeliverySystem.Services.Commands;

public sealed class DeliveryCommandInvoker
{
    private readonly Stack<IDeliveryCommand> _undoStack = new();
    private readonly Stack<IDeliveryCommand> _redoStack = new();
    private readonly List<string> _history = new();

    public IReadOnlyList<string> History => _history.AsReadOnly();
    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public void Execute(IDeliveryCommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
        _history.Add($"EXEC: {command.Description}");
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
            throw new InvalidOperationException("Nothing to undo.");

        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
        _history.Add($"UNDO: {command.Description}");
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
            throw new InvalidOperationException("Nothing to redo.");

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        _history.Add($"REDO: {command.Description}");
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        _history.Clear();
    }
}
