﻿using System.Collections.Generic;

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
        /// <returns>
        ///     <see langword="true"/> if service is available, <see langword="false"/> if it isn't, <see langword="null"/> if parameters not found in <paramref name="settings"/> 
        /// </returns>
        void Check(Dictionary<string, string[]> checkParams);
    }
}
