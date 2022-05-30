using System;

using AvailabilityChecker.Checks;
using AvailabilityChecker.Email;
using AvailabilityChecker.Logging;

using CommandLine;

namespace AvailabilityChecker
{
    internal class CommandLineParameters
    {
        [Option("show", Default = false, HelpText = "Show results from previous checks")]
        public bool ShowResults { get; set; }
        
        [Option('s', "settings", MetaValue = "PATH", HelpText = "Set path to settings", Default = "settings.json")]
        public string SettingsPath { get; set; }
        
        [Option('r', "results", MetaValue = "PATH", HelpText = "Set path to results", Default = "results.json")]
        public string ResultsPath { get; set; }
    }

    internal class Program
    {
        private static readonly ICheck[] _checks = { new WebsiteChecker(), new DbChecker() };
        
        private static string SettingsPath;
        private static string ResultsPath;

        private static string SETTINGS_LOAD_ERROR_TEXT => $"Failed to load settings from {SettingsPath}";
        private static string RESULTS_LOAD_ERROR_TEXT => $"Failed to load results from {ResultsPath}";
        private const string EXIT_TEXT = "Press any key to continue...";

        private static Settings _settings;
        private static ILogger _logger;

        private static void Main(string[] args)
        {
            _logger = new ConsoleLogger();

            var parserResult = Parser.Default.ParseArguments<CommandLineParameters>(args);
            CommandLineParameters cmdParams = parserResult.Value;
            if (cmdParams is null)
            {
                return;
            }
            SettingsPath = cmdParams.SettingsPath;
            ResultsPath = cmdParams.ResultsPath;

            _logger?.Log("Loading settings...");
            if (!cmdParams.ShowResults && !TryLoadSettings())
            {
                _logger?.Log(SETTINGS_LOAD_ERROR_TEXT);
                FinishUI();
                return;
            }
            _logger?.Log("Settings loaded!");

            if (!cmdParams.ShowResults)
            {
                _logger?.Log("Performing checks...");
                CheckResultsCollection results = PerformChecks();
                _logger?.Log("Checks done!");

                _logger?.Log("Saving results...");
                SaveResults(results);
                _logger?.Log("Results saved!");

                _logger?.Log("Sending emails...");
                SendEmails(results);
                _logger?.Log("Done!");
            }
            ShowResults();
            FinishUI();
        }

        private static bool TryLoadSettings()
        {
            try
            {
                _settings = Settings.Load(SettingsPath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static void SendEmails(CheckResultsCollection results)
        {
            string message = results.ToString();
            string[] attachments = { ResultsPath };
            EmailSender sender;
            try
            {
                sender = new(_settings.EmailSender, _settings.EmailReceivers, _logger);
            }
            catch (Exception e)
            {
                _logger?.Log(e.Message);
                return;
            }
            sender.Send(_settings.EmailMessageSubject, message, attachments);
        }

        private static CheckResultsCollection PerformChecks()
        {
            CheckResultsCollection results = new();
            foreach (var check in _checks)
            {
                check.Check(_settings.Checks);
                results.AddRange(check.Results);
            }
            return results;           
        }

        private static void SaveResults(CheckResultsCollection results)
        {
            results.SaveAs(ResultsPath);
        }

        private static void ShowResults()
        {
            CheckResultsCollection results;
            try
            {
                results = CheckResultsCollection.Load(ResultsPath);
            }
            catch
            {
                _logger?.Log(RESULTS_LOAD_ERROR_TEXT);
                return;
            }
            _logger?.Log(results.ToString());
        }

        private static void FinishUI()
        {
            _logger?.Log(EXIT_TEXT);
            Console.ReadKey();
        }
    }
}