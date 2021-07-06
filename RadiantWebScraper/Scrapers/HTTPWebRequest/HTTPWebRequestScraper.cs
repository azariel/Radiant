//using System.Net;
//using System.Net.Mime;

//namespace Radiant.WebScraper.Scrapers.HTTPWebRequest
//{
//    public class HTTPWebRequestScraper : IScraper
//    {
//        // ********************************************************************
//        //                            Public
//        // ********************************************************************
//        public string GetDOMFromUrl(string aUrl)
//        {
//            HttpWebRequest _HttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
//            _HttpWebRequest.UserAgent = UserAgent;
//            _HttpWebRequest.AllowWriteStreamBuffering = true;
//            _HttpWebRequest.ProtocolVersion = HttpVersion.Version11;
//            _HttpWebRequest.AllowAutoRedirect = true;
//            _HttpWebRequest.ContentType = ContentType;
//            _HttpWebRequest.PreAuthenticate = true;

//        }
//    }
//}
