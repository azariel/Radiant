using System.Collections.Generic;
using System.Net.Http;
using Radiant.Common.Diagnostics;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;
using Radiant.WebScraper.RadiantWebScraper.Scrapers;

namespace Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Automatic.HttpClient
{
    public class HttpClientDOMTargetScraper : IScraperTarget
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems)
        {
            try
            {
                using var _Handler = new HttpClientHandler();
                _Handler.UseDefaultCredentials = true;
                using System.Net.Http.HttpClient _HttpClient = new System.Net.Http.HttpClient(_Handler);

                //_HttpClient.DefaultRequestHeaders.UserAgent.Add(new("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11", null));
                _HttpClient.DefaultRequestHeaders.Clear();
                _HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/json");
                _HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:98.0) Gecko/20100101 Firefox/98.0");


                this.DOM = _HttpClient.GetStringAsync(aUrl).Result;
            }
            catch (HttpRequestException _Ex)
            {
                LoggingManager.LogToFile("1d42fb20-0af0-4323-baf0-dd76aeee7ead", "HttpClient exception.", _Ex);
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string DOM { get; set; }
    }
}
