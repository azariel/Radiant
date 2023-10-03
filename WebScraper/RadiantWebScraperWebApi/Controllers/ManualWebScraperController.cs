using System.Net;
using Microsoft.AspNetCore.Mvc;
using Radiant.WebScraper.RadiantWebScraper;
using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.RadiantWebScraper.Scrapers.Manual;

namespace Radiant.WebScraper.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ManualWebScraperController : ControllerBase
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [HttpGet("{Url}")]
        [ActionName("DOM")]
        public async Task<ActionResult<string>> GetDOMAsync(string Url)
        {
            if (string.IsNullOrWhiteSpace(Url))
                return BadRequest($"Argument {nameof(Url)} is required.");

            string _DecodedUrl = WebUtility.UrlDecode(Url);
            // TODO: format aUrl, pad with "http://", etc.

            var _ManualScraper = new ManualScraper();
            var _DomTargetScraper = new ManualDOMTargetScraper();
            await _ManualScraper.GetTargetValueFromUrlAsync(Browser.Firefox, _DecodedUrl, _DomTargetScraper, null, null);

            if (!string.IsNullOrWhiteSpace(_DomTargetScraper.DOM))
                return new ActionResult<string>(_DomTargetScraper.DOM);

            return StatusCode((int)HttpStatusCode.InternalServerError, "DOM couldn't be fetched. Unhandled error.");
        }
    }
}
