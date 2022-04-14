using System.Web;
using Radiant.Common.HttpClients.RestClient;

namespace RadiantClientWebScraper
{
    // TODO: make this generic, it's almost the same as AutomaticWebScraperClient
    public static class ManualWebScraperClient
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private const string URL_PREFIX_WITH_URL_PARAM = "https://localhost:6501/ManualWebScraper?Url=";// TODO: revisit this http vs https and port

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string GetDOM(string aRelativeUrl)
        {
            var _Client = new HttpRestClient();
            string _EncodedUrlParam = HttpUtility.UrlEncode(aRelativeUrl);
            return _Client.Get($"{URL_PREFIX_WITH_URL_PARAM}{_EncodedUrlParam}");
        }
    }
}
