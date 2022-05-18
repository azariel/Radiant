using Microsoft.AspNetCore.Mvc;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Automatic.Selenium;
using Radiant.WebScraper.Scrapers.Automatic.Selenium;
using System.Net;

namespace RadiantWebScraperWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AutomaticWebScraperController : ControllerBase
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [HttpGet("{Url}")]
        [ActionName("DOM")]
        public async Task<ActionResult<string>> GetDOMAsync(string Url)
        {
            if (string.IsNullOrWhiteSpace(Url))
                throw new Exception($"Argument {nameof(Url)} is required.");

            string _DecodedUrl = WebUtility.UrlDecode(Url);

            // TODO: format aUrl, pad with "http://", etc ?

            var _SeleniumScraper = new SeleniumScraper();
            var _DomScraper = new SeleniumDOMTargetScraper();

            await _SeleniumScraper.GetTargetValueFromUrlAsync(Browser.Firefox, _DecodedUrl, _DomScraper, aScraperItemsParser: null, aParserItems: null);

            if (!string.IsNullOrWhiteSpace(_DomScraper.DOM))
                return new ActionResult<string>(_DomScraper.DOM);

            return StatusCode((int)HttpStatusCode.InternalServerError, "DOM couldn't be fetched. Unhandled error.");
        }
    }
}
