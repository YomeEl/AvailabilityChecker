using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using AvailabilityChecker.Email;

namespace AvailabilityChecker
{
    public struct Settings
    {
        public Sender EmailSender;
        public Receiver[] EmailReceivers;
        public string EmailMessageSubject;
        public Dictionary<string, string[]> Checks;

        public void SaveAs(string path)
        {
            JsonSerializer serializer = new();
            using StreamWriter sw = new(path);
            using JsonTextWriter writer = new(sw) { Formatting = Formatting.Indented };
            serializer.Serialize(writer, this);
        }

        public static Settings Load(string path)
        {
            JsonSerializer serializer = new();
            using StreamReader sr = new(path);
            using JsonTextReader reader = new(sr);
            return serializer.Deserialize<Settings>(reader);
        }
    }
}
