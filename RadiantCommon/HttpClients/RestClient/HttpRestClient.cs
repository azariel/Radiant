using System;
using System.Net.Http;

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
            using var client = new HttpClient();
            HttpResponseMessage _Response = client.GetAsync(aUrl).Result;

            if (_Response.IsSuccessStatusCode)
                return _Response.Content.ReadAsStringAsync().Result;

            throw new Exception("Unhandled exception");// TODO: handle errors
        }
    }
}
