using System.Text;

namespace DeliverySystem.Domain.Templates;

/// <summary>
/// Template Method base. Generate() is the fixed algorithm; subclasses customize the four
/// content steps. Hooks (IncludeMetadata) let subclasses opt in/out of optional sections
/// without changing the structure.
/// </summary>
public abstract class ReceiptGenerator
{
    public string Generate()
    {
        var sb = new StringBuilder();
        WriteHeader(sb);
        sb.AppendLine();
        WriteLines(sb);
        sb.AppendLine();
        WriteSummary(sb);

        if (IncludeMetadata)
        {
            sb.AppendLine();
            WriteMetadata(sb);
        }

        sb.AppendLine();
        WriteFooter(sb);
        return sb.ToString();
    }

    protected abstract void WriteHeader(StringBuilder sb);
    protected abstract void WriteLines(StringBuilder sb);
    protected abstract void WriteSummary(StringBuilder sb);

    protected virtual void WriteFooter(StringBuilder sb)
    {
        sb.AppendLine("-- end of receipt --");
    }

    protected virtual bool IncludeMetadata => false;
    protected virtual void WriteMetadata(StringBuilder sb) { }
}
