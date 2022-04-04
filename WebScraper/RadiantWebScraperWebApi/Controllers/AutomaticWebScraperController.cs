using Microsoft.AspNetCore.Mvc;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Automatic.Selenium;
using Radiant.WebScraper.Scrapers.Automatic.Selenium;

namespace RadiantWebScraperWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutomaticWebScraperController : ControllerBase
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [HttpGet(Name = "GetDOMByAutomaticWebScraper")]
        public string GetDOM(string aUrl)
        {
            if(string.IsNullOrWhiteSpace(aUrl))
                throw new Exception($"Argument {nameof(aUrl)} is required.");

            // TODO: format aUrl, pad with "http://", etc.

            SeleniumScraper _SeleniumScraper = new SeleniumScraper();
            var _DomScraper = new SeleniumDOMTargetScraper();
            _SeleniumScraper.GetTargetValueFromUrl(Browser.Firefox, aUrl, _DomScraper, aScraperItemsParser: null, aParserItems: null);

            return _DomScraper.DOM;
        }
    }
}
