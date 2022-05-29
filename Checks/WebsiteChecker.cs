using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AvailabilityChecker.Checks
{
    public class WebsiteChecker : ICheck
    {
        public CheckResultsCollection Results { get; private set; }

        private const string KEY = "WebsiteCheck_URLs";
        private const string DISPLAY_NAME = "Website check";

        public WebsiteChecker()
        {
            Results = new();
        }

        public void Check(Dictionary<string, string[]> checkParams)
        {
            if (!checkParams.ContainsKey(KEY))
            {
                return;
            }

            string[] URLs = checkParams[KEY];
            foreach (var url in URLs)
            {
                UriBuilder uriBuilder = new(url);
                Uri uri = uriBuilder.Uri;
                bool result = CheckUri(uri);
                AddResult(result, uri.ToString());
            }
        }

        private static bool CheckUri(Uri uri)
        {
            using var client = new HttpClient();
            HttpRequestMessage message = new(HttpMethod.Get, uri);
            bool isAvailable;
            try
            {
                using var response = client.Send(message);
                isAvailable = response?.IsSuccessStatusCode == true;
            }
            catch
            {
                isAvailable = false;
            }
            return isAvailable;
        }

        private void AddResult(bool isAvailable, string url)
        {
            string message = $"Resource {url} is {(isAvailable ? "" : "not ")}available";
            Results.Add(new(DISPLAY_NAME, message));
        }
    }
}
