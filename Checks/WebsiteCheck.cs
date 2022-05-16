using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AvailabilityChecker.Checks
{
    public class WebsiteCheck : ICheck
    {
        public CheckResult Result { get; private set; } = null;

        private const string KEY = "WebsiteCheck_URL";
        private const string DISPLAY_NAME = "Website check";

        public bool? Check(Dictionary<string, string> checkParams)
        {
            if (!checkParams.ContainsKey(KEY))
            {
                return null;
            }

            UriBuilder uriBuilder = new(checkParams[KEY]);
            Uri uri = uriBuilder.Uri;
            using var client = new HttpClient();
            HttpRequestMessage message = new(HttpMethod.Get, uri);
            bool isAvailable;
            try
            {
                using var response = client.Send(message);
                isAvailable = response is not null && response.IsSuccessStatusCode;
            }
            catch
            {
                isAvailable = false;
            }
            
            SetStatus(isAvailable, uri.ToString());
            return isAvailable;
        }

        private void SetStatus(bool isAvailable, string url)
        {
            string message = $"Resource {url} is {(isAvailable ? "" : "not ")}available";
            Result = new(DISPLAY_NAME, message);
        }
    }
}
