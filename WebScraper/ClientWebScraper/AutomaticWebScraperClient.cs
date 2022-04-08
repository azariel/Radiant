using Radiant.Common.HttpClients.RestClient;

namespace ClientWebScraper
{
    public static class AutomaticWebScraperClient
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private const string URL_PREFIX_WITH_URL_PARAM = "https://localhost:6501/AutomaticWebScraper?Url=";// TODO: revisit this http vs https and port

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string Get(string aRelativeUrl)
        {
            var _Client = new HttpRestClient();
            return _Client.Get($"{URL_PREFIX_WITH_URL_PARAM}{aRelativeUrl}");
        }
    }
}
