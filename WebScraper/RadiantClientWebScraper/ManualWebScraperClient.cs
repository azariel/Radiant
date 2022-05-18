using Radiant.Common.HttpClients.RestClient;
using System.Net;

namespace RadiantClientWebScraper
{
    // TODO: make this generic, it's almost the same as AutomaticWebScraperClient
    public static class ManualWebScraperClient
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private const string URL_PREFIX_ASYNC_WITH_URL_PARAM = "https://localhost:6501/api/ManualWebScraper/DOM/";// TODO: revisit this http vs https and port

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static async Task<string> GetDOMAsync(string aRelativeUrl)
        {
            var _Client = new HttpRestClient();
            string _EncodedUrlParam = WebUtility.UrlEncode(aRelativeUrl);
            return await _Client.GetAsync($"{URL_PREFIX_ASYNC_WITH_URL_PARAM}{_EncodedUrlParam}", 600000);// 10 min timeout as we're in a manual fetch
        }
    }
}
