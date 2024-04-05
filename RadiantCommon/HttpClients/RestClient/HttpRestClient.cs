using System;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
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
        [Obsolete("Use GetAsync instead.")]// TODO: remove this function ?
        public string Get(string aUrl, int aMsTimeOut = 30000)
        {
            using var _Client = new HttpClient();
            _Client.Timeout = new TimeSpan(0, 0, 0, 0, aMsTimeOut);

            try
            {
                HttpResponseMessage _Response = _Client.GetAsync(aUrl).Result;

                if (_Response.IsSuccessStatusCode)
                    return _Response.Content.ReadAsStringAsync().Result;

                LoggingManager.LogToFile("43b47bd1-9976-4d13-ad57-a51304529c24", $"WebScraper Api returned an error code [{_Response.StatusCode}]. Content = [{_Response.Content}]");
                throw new Exception($"WebScraper Api returned an error code [{_Response.StatusCode}].");
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

        /// <summary>
        /// Asynchronous get
        /// </summary>
        public async Task<string> GetAsync(string aUrl, int aMsTimeOut = 30000, bool aIgnoreCertificateErrors = false)
        {
            using var _HttpClientHandler = new HttpClientHandler();
            _HttpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => sslPolicyErrors == SslPolicyErrors.None || aIgnoreCertificateErrors;

            using var _Client = new HttpClient(_HttpClientHandler);
            _Client.Timeout = new TimeSpan(0, 0, 0, 0, aMsTimeOut);

            try
            {
                HttpResponseMessage _Response = await _Client.GetAsync(aUrl);

                if (_Response.IsSuccessStatusCode)
                    return await _Response.Content.ReadAsStringAsync();
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

        public async Task<string> PostAsync(string aUrl, string jsonPayload, int aMsTimeOut = 30000, bool aIgnoreCertificateErrors = false)
        {
            using var _HttpClientHandler = new HttpClientHandler();
            _HttpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => sslPolicyErrors == SslPolicyErrors.None || aIgnoreCertificateErrors;

            using var _Client = new HttpClient(_HttpClientHandler);
            _Client.Timeout = new TimeSpan(0, 0, 0, 0, aMsTimeOut);

            try
            {
                HttpResponseMessage _Response = await _Client.PostAsync(aUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                string responseMessageContent = await _Response.Content.ReadAsStringAsync();

                if (_Response.IsSuccessStatusCode)
                    return responseMessageContent;

                string errorMessage = $"Couldn't post async to dependent service. Url = [{aUrl}]. HTTP Response Code = [{_Response.StatusCode}]. Response Message = [{responseMessageContent}].";
                LoggingManager.LogToFile("b423f45b-7032-4eb4-b674-6296a95783db", errorMessage);
                throw new Exception(errorMessage);
            }
            catch (AggregateException _AggregateException) when (_AggregateException.InnerExceptions.Any(a => a.GetType() == typeof(HttpRequestException)))
            {
                LoggingManager.LogToFile("157c8016-86d0-41bf-8649-3a176bb6e3a8", $"Couldn't query dependent service. Url = [{aUrl}]", _AggregateException);
                throw;
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("74f24fa2-c994-40ec-9703-007bd0739cef", $"Unhandled exception when querying WebScraper Api querying url [{aUrl}].", _Ex);
                throw;
            }

            throw new Exception($"Unhandled exception querying url [{aUrl}].");// TODO: wrap error
        }

        public async Task<string> PatchAsync(string aUrl, string jsonPayload, int aMsTimeOut = 30000, bool aIgnoreCertificateErrors = false)
        {
            using var _HttpClientHandler = new HttpClientHandler();
            _HttpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => sslPolicyErrors == SslPolicyErrors.None || aIgnoreCertificateErrors;

            using var _Client = new HttpClient(_HttpClientHandler);
            _Client.Timeout = new TimeSpan(0, 0, 0, 0, aMsTimeOut);

            try
            {
                HttpResponseMessage _Response = await _Client.PatchAsync(aUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

                if (_Response.IsSuccessStatusCode)
                    return await _Response.Content.ReadAsStringAsync();
            }
            catch (AggregateException _AggregateException) when (_AggregateException.InnerExceptions.Any(a => a.GetType() == typeof(HttpRequestException)))
            {
                LoggingManager.LogToFile("6b477a34-a5d7-47e8-86f7-4fb8caa0f6ff", $"Couldn't query dependent service. Url = [{aUrl}]", _AggregateException);
                throw;
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("71601440-192c-4479-bbab-4a67e292338d", $"Unhandled exception when querying WebScraper Api querying url [{aUrl}].", _Ex);
                throw;
            }

            throw new Exception($"Unhandled exception querying url [{aUrl}].");// TODO: wrap error
        }
    }
}
