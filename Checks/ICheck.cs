using System.Collections.Generic;

namespace AvailabilityChecker.Checks
{
    public interface ICheck
    {
        /// <summary>
        /// Last check status
        /// </summary>
        CheckResultsCollection Results { get; }

        /// <summary>
        /// Performs check
        /// </summary>
        /// <param name="checkParams">Collection of check parameters</param>
        void Check(Dictionary<string, string[]> checkParams);
    }
}
