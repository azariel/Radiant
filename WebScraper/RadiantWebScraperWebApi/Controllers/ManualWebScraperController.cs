using Microsoft.AspNetCore.Mvc;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.Scrapers.Manual;

namespace RadiantWebScraperWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManualWebScraperController : ControllerBase
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [HttpGet(Name = "GetDOMByManualWebScraper")]
        public string GetDOM(string Url)
        {
            if(string.IsNullOrWhiteSpace(Url))
                throw new Exception($"Argument {nameof(Url)} is required.");

            // TODO: format aUrl, pad with "http://", etc.

            ManualScraper _ManualScraper = new ManualScraper();
            var _DomScraper = new ManualDOMTargetScraper();
            _ManualScraper.GetTargetValueFromUrl(Browser.Firefox, Url, _DomScraper, null, null);

            return _DomScraper.DOM;
        }
    }
}
