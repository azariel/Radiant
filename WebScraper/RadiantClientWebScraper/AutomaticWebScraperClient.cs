using Radiant.Common.HttpClients.RestClient;
using System.Net;

namespace RadiantClientWebScraper
{
    public static class AutomaticWebScraperClient
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private const string URL_PREFIX_WITH_URL_PARAM = "https://localhost:6501/api/AutomaticWebScraper/DOM/";// TODO: revisit this http vs https and port

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string GetDOM(string aRelativeUrl)
        {
            // TODO: async vs sync calls
            var _Client = new HttpRestClient();
            string _EncodedUrlParam = WebUtility.UrlEncode(aRelativeUrl);
            return _Client.Get($"{URL_PREFIX_WITH_URL_PARAM}{_EncodedUrlParam}");
        }
    }
}