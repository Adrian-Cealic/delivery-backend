using System.Text;
using DeliverySystem.Interfaces.Reports;

namespace DeliverySystem.Infrastructure.Reports;

public sealed class ConsoleReportRenderer : IReportRenderer
{
    public string Render(string title, IReadOnlyDictionary<string, string> data)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Report title cannot be empty.", nameof(title));

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var sb = new StringBuilder();
        sb.AppendLine(new string('=', 50));
        sb.AppendLine($"  {title}");
        sb.AppendLine(new string('=', 50));

        foreach (var kvp in data)
        {
            sb.AppendLine($"  {kvp.Key,-25} {kvp.Value}");
        }

        sb.AppendLine(new string('-', 50));
        return sb.ToString();
    }
}
