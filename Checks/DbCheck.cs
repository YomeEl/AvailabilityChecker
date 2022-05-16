using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace AvailabilityChecker.Checks
{
    class DbCheck : ICheck
    {
        public CheckResult Result { get; private set; } = null;

        private const string KEY = "DbCheck_ConnectionString";
        private const string DISPLAY_NAME = "MS SQL check";

        public bool? Check(Dictionary<string, string> checkParams)
        {
            if (!checkParams.ContainsKey(KEY))
            {
                return null;
            }

            string connectionString = checkParams[KEY];

            bool result = true;
            try
            {
                using SqlConnection connection = new(connectionString);
                connection.Open();
            }
            catch (ArgumentException e)
            {
                Result = new CheckResult(DISPLAY_NAME, e.Message);
                return false;
            }
            catch (SqlException)
            {
                result = false;
            }

            SetStatus(result);
            return result;
        }

        private void SetStatus(bool isAvailable)
        {
            string message = $"Database with provided connection string is {(isAvailable ? "" : "not ")}available";
            Result = new CheckResult(DISPLAY_NAME, message);
        }
    }
}
