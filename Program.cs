using System;
using AvailabilityChecker.Checks;
using AvailabilityChecker.Email;

namespace AvailabilityChecker
{
    class Program
    {
        private static readonly ICheck[] _checks = { new WebsiteChecker(), new DbChecker() };
        
        private const string SETTINGS_PATH = "settings.json";
        private const string RESULTS_PATH = "results.json";

        private const string SETTINGS_LOAD_ERROR_TEXT = "Failed to load settings from " + SETTINGS_PATH;
        private const string RESULTS_LOAD_ERROR_TEXT = "Failed to load results from " + RESULTS_PATH;

        private const string EXIT_TEXT = "Press any key to exit";

        private const string MESSAGE_SUBJECT = "Check results";

        private const string SHOW_RESULTS_PARAMETER = "--show";
        

        private static Settings settings;

        static void Main(string[] args)
        {
            if (!(args.Length > 0 && args[0] == SHOW_RESULTS_PARAMETER))
            {
                PerformChecks();
            }
            ShowResults();
            FinishUI();
        }

        static void SendEmails(CheckResultsCollection results)
        {
            string message = results.ToString();
            string[] attachments = { RESULTS_PATH };
            MailSender.SendEmails(settings.Emails, MESSAGE_SUBJECT, message, attachments);
        }
        
        static void PerformChecks()
        {
            Console.WriteLine("Loading settings...");
            try
            {
                settings = Settings.Load(SETTINGS_PATH);
            }
            catch
            {
                Console.WriteLine(SETTINGS_LOAD_ERROR_TEXT);
                Console.WriteLine(EXIT_TEXT);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Settings loaded!");

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
            results.SaveAs(RESULTS_PATH);
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
                results = CheckResultsCollection.Load(RESULTS_PATH);
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
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}