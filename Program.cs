using System;
using AvailabilityChecker.Checks;
using AvailabilityChecker.Email;
using AvailabilityChecker.Logging;
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

        [Option('s', "silent", Default = false, HelpText = "Do not print to the console")]
        public bool Silent { get; set; }
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

        private static ILogger logger;

        static void Main(string[] args)
        {
            logger = new ConsoleLogger();

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
                logger?.Log(SETTINGS_LOAD_ERROR_TEXT);
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
            logger?.Log("Loading settings...");
            try
            {
                settings = Settings.Load(SettingsPath);
            }
            catch
            {
                return false;
            }
            logger?.Log("Settings loaded!");
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
            logger?.Log("Performing checks...");
            CheckResultsCollection results = new();
            foreach (var check in _checks)
            {
                check.Check(settings.Checks);
                if (check.Results is not null)
                {
                    results.AddRange(check.Results);
                }
            }
            logger?.Log("Checks done!");

            logger?.Log("Saving settings...");
            results.SaveAs(ResultsPath);
            logger?.Log("Settings saved!");

            logger?.Log("Sending emails...");
            SendEmails(results);
            logger?.Log("Done!");
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
                logger?.Log(RESULTS_LOAD_ERROR_TEXT);
                return;
            }
            logger?.Log(results.ToString());
        }

        static void FinishUI()
        {
            logger?.Log(EXIT_TEXT);
            Console.ReadKey();
        }
    }
}