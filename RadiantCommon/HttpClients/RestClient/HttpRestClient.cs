using System;
using System.Linq;
using System.Net.Http;
using Radiant.Common.Diagnostics;

namespace Radiant.Common.HttpClients.RestClient
{
    // TODO: implement async
    public class HttpRestClient
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// Synchronous get
        /// </summary>
        public string Get(string aUrl)
        {
            using var _Client = new HttpClient();

            try
            {
                HttpResponseMessage _Response = _Client.GetAsync(aUrl).Result;

                if (_Response.IsSuccessStatusCode)
                    return _Response.Content.ReadAsStringAsync().Result;
            }
            catch (AggregateException _AggregateException) when (_AggregateException.InnerExceptions.Any(a => a.GetType() == typeof(HttpRequestException)))
            {
                LoggingManager.LogToFile("270b1291-8da6-4f20-8f73-30464927fe6e", "Couldn't query WebScraper micro service.", _AggregateException);
                throw;
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("16f4baea-ecba-4631-a4a0-65a2653cf63d", "Unhandled exception when querying WebScraper Api.", _Ex);
                throw;
            }

            throw new Exception("Unhandled exception");// TODO: wrap error
        }
    }
}
