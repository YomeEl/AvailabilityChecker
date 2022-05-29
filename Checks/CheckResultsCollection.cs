using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace AvailabilityChecker.Checks
{
    public class CheckResultsCollection
    {
        public List<CheckResult> Results { get; } = new();

        public void Add(CheckResult result)
        {
            Results.Add(result);
        }

        public void AddRange(IEnumerable<CheckResult> results)
        {
            Results.AddRange(results);
        }

        public void AddRange(CheckResultsCollection checkResults)
        {
            Results.AddRange(checkResults.Results);
        }

        public void SaveAs(string path)
        {
            JsonSerializer serializer = new();
            using StreamWriter sw = new(path);
            using JsonTextWriter writer = new(sw) { Formatting = Formatting.Indented };
            serializer.Serialize(writer, this);
        }

        public static CheckResultsCollection Load(string path)
        {
            JsonSerializer serializer = new();
            using StreamReader sr = new(path);
            using JsonTextReader reader = new(sr);
            return serializer.Deserialize<CheckResultsCollection>(reader);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("Result of checks");
            foreach (var result in Results)
            {
                sb.AppendLine("=====");
                sb.AppendLine(result.ToString());
            }
            sb.AppendLine("====");
            return sb.ToString();
        }
    }
}
