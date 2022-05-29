using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace AvailabilityChecker.Checks
{
    class DbChecker : ICheck
    {
        public CheckResultsCollection Results { get; private set; }

        private const string KEY = "DbCheck_ConnectionStrings";
        private const string DISPLAY_NAME = "MS SQL check";

        public DbChecker()
        {
            Results = new();
        }

        public void Check(Dictionary<string, string[]> checkParams)
        {
            if (!checkParams.ContainsKey(KEY))
            {
                return;
            }

            string[] connectionStrings = checkParams[KEY];
            foreach (var cs in connectionStrings)
            {
                bool result = CheckConnection(cs);
                AddResult(result);
            }
        }

        private static bool CheckConnection(string connectionString)
        {
            bool result = true;
            try
            {
                using SqlConnection connection = new(connectionString);
                connection.Open();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void AddResult(bool isAvailable)
        {
            string message = $"Database with provided connection string is {(isAvailable ? "" : "not ")}available";
            Results.Add(new(DISPLAY_NAME, message));
        }
    }
}
