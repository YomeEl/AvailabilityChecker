using System;
using AvailabilityChecker.Checks;
using AvailabilityChecker.Email;
using CommandLine;

namespace AvailabilityChecker
{
    public class CommandLineParameters
    {
        [Option("show", Default = false, HelpText = "Show results from previous checks")]
        public bool ShowResults { get; set; }
        
        [Option('s', "settings", MetaValue = "PATH", HelpText = "Set path to settings", Default = "settings.json")]
        public string SettingsPath { get; set; }
        
        [Option('r', "results", MetaValue = "PATH", HelpText = "Set path to results", Default = "results.json")]
        public string ResultsPath { get; set; }
    }

    class Program
    {
        private static readonly ICheck[] _checks = { new WebsiteChecker(), new DbChecker() };
        
        private static string SettingsPath;
        private static string ResultsPath;

        private static string SETTINGS_LOAD_ERROR_TEXT = "Failed to load settings from " + SettingsPath;
        private static string RESULTS_LOAD_ERROR_TEXT = "Failed to load results from " + ResultsPath;

        private const string EXIT_TEXT = "Press any key to continue...";

        private const string MESSAGE_SUBJECT = "Check results";

        private static Settings settings;

        static void Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<CommandLineParameters>(args);
            CommandLineParameters cmdParams = parserResult.Value;
            if (cmdParams is null)
            {
                return;
            }
            SettingsPath = cmdParams.SettingsPath;
            ResultsPath = cmdParams.ResultsPath;
            if (!cmdParams.ShowResults && !TryLoadSettings())
            {
                Console.WriteLine(SETTINGS_LOAD_ERROR_TEXT);
                FinishUI();
                return;
            }
            if (!cmdParams.ShowResults)
            {
                PerformChecks();
            }
            ShowResults();
            FinishUI();
        }

        static bool TryLoadSettings()
        {
            Console.WriteLine("Loading settings...");
            try
            {
                settings = Settings.Load(SettingsPath);
            }
            catch
            {
                return false;
            }
            Console.WriteLine("Settings loaded!");
            return true;
        }

        static void SendEmails(CheckResultsCollection results)
        {
            string message = results.ToString();
            string[] attachments = { ResultsPath };
            MailSender.SendEmails(settings.Emails, MESSAGE_SUBJECT, message, attachments);
        }
        
        static void PerformChecks()
        {
            Console.WriteLine("Performing checks...");
            CheckResultsCollection results = new();
            foreach (var check in _checks)
            {
                check.Check(settings.Checks);
                if (check.Results is not null)
                {
                    results.AddRange(check.Results);
                }
            }
            Console.WriteLine("Checks done!");

            Console.WriteLine("Saving settings...");
            results.SaveAs(ResultsPath);
            Console.WriteLine("Settings saved!");

            Console.WriteLine("Sending emails...");
            SendEmails(results);
            Console.WriteLine("Done!");
        }

        static void ShowResults()
        {
            CheckResultsCollection results;

            try
            {
                results = CheckResultsCollection.Load(ResultsPath);
            }
            catch
            {
                Console.WriteLine(RESULTS_LOAD_ERROR_TEXT);
                return;
            }
            Console.WriteLine(results.ToString());
        }

        static void FinishUI()
        {
            Console.WriteLine(EXIT_TEXT);
            Console.ReadKey();
        }
    }
}