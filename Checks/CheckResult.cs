using System.Text;

namespace AvailabilityChecker.Checks
{
    public struct CheckResult
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

        public CheckResult(string name, string message, string details = null)
        {
            Name = name;
            Message = message;
            Details = details;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("Check:");
            sb.AppendLine($"\t{Name}");
            sb.AppendLine("Result:");
            sb.AppendLine($"\t{Message}");
            if (Details is not null)
            {
                sb.AppendLine("Details:");
                sb.AppendLine($"\t{Details}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}