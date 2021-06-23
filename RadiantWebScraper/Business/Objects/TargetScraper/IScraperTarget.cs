using System.Collections.Generic;
using Radiant.WebScraper.Parsers.DOM;

namespace Radiant.WebScraper.Business.Objects.TargetScraper
{
    public interface IScraperTarget
    {
        void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<DOMParserItem> aDOMParserItems);
    }
}
