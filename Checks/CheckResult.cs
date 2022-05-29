using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvailabilityChecker.Checks
{
    public struct CheckResult
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public CheckResult(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Check:");
            sb.AppendLine($"\t{Name}");
            sb.AppendLine($"Result:");
            sb.Append($"\t{Message}");
            return sb.ToString();
        }
    }
}