using System;
using System.Net;

namespace Radiant.Common.Exceptions
{
    public class ApiException : Exception
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }

        public ApiException(HttpStatusCode code, string message, Exception originalException = null) : base(message, originalException)
        {
            Code = code;
            Message = message;
        }
    }
}
