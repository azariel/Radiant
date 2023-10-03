using System.Net;
using Microsoft.AspNetCore.Mvc;
using Radiant.WebScraper.RadiantWebScraper;
using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Automatic.Selenium;
using Radiant.WebScraper.RadiantWebScraper.Scrapers.Automatic.Selenium;

namespace Radiant.WebScraper.WebApi.Controllers
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
