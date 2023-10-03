using System.Net;
using Radiant.Common.HttpClients.RestClient;

namespace Radiant.WebScraper.RadiantClientWebScraper
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
        public static async Task<string> GetDOMAsync(string aRelativeUrl)
        {
            // TODO: async vs sync calls
            var _Client = new HttpRestClient();
            string _EncodedUrlParam = WebUtility.UrlEncode(aRelativeUrl);
            return await _Client.GetAsync($"{URL_PREFIX_WITH_URL_PARAM}{_EncodedUrlParam}", 120000);// 2 min
        }
    }
}