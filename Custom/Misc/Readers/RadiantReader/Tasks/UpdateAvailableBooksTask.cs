using System.Collections.Generic;
using Radiant.Common.Tasks.Triggers;
using RadiantReader.DataBase;
using RadiantReader.Managers;

namespace RadiantReader.Tasks
{
    /// <summary>
    /// Fetch available books / fictions from the internet and store them in storage.
    /// Only fetch summary and information about the fic, not the actual entire fic.
    /// </summary>
    public class UpdateAvailableBooksTask : RadiantTask
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void FetchBooksOnLandingPage()
        {
            // Load all DataBase
            List<RadiantReaderHostModel> _HostBooks = StorageManager.LoadBooks(true);

            //SeleniumScraper _Scraper = new SeleniumScraper();
            //SeleniumDOMTargetScraper _DOMTargetScraper = new SeleniumDOMTargetScraper();

            //foreach (RadiantReaderHostModel _Host in _HostBooks)
            //{
            //    _Scraper.GetTargetValueFromUrl(Browser.Firefox, _Host.HostLandingPage, _DOMTargetScraper, null, null);
            //    ParseBooksFromDOMLandingPage(_DOMTargetScraper.DOM);
            //}
        }

        private void ParseBooksFromDOMLandingPage(string aDOM)
        {
            // TODO: by domain. ex: parse fanfiction, parse archiveOfOurOwn, etc etc
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            FetchBooksOnLandingPage();
        }
    }
}
