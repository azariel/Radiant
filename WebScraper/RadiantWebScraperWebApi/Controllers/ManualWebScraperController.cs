using Microsoft.AspNetCore.Mvc;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.Scrapers.Manual;

namespace RadiantWebScraperWebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ManualWebScraperController : ControllerBase
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [HttpGet]
        [ActionName("DOM")]
        public async Task<string> GetDOMAsync(string Url)
        {
            // TODO: create an helper that'll do it for us
            if (string.IsNullOrWhiteSpace(Url))
                throw new Exception($"Argument {nameof(Url)} is required.");

            // TODO: format aUrl, pad with "http://", etc.

            ManualScraper _ManualScraper = new ManualScraper();
            var _DomScraper = new ManualDOMTargetScraper();
            IScraperTarget? _Task = await _ManualScraper.GetTargetValueFromUrlAsync(Browser.Firefox, Url, _DomScraper, null, null);

            if (_Task is ManualDOMTargetScraper _ReturningManualDOMTargetScraper)
                return _ReturningManualDOMTargetScraper.DOM;

            return null;
        }
    }
}
