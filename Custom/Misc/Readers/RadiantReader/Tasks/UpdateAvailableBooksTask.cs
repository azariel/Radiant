using System.Collections.Generic;
using Radiant.Common.Tasks.Triggers;
using RadiantClientWebScraper;
using RadiantReader.DataBase;
using RadiantReader.Managers;
using RadiantReader.Utils;

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

            foreach (RadiantReaderHostModel _Host in _HostBooks)
            {
                string _DOM = AutomaticWebScraperClient.GetDOM(_Host.HostLandingPage);
                ParseBooksFromDOMLandingPage(_DOM);
            }
        }

        private void ParseBooksFromDOMLandingPage(string aDOM)
        {
            // TODO: by domain. ex: parse fanfiction, parse archiveOfOurOwn, etc etc
            var _Books = DOMUtils.ParseBooksFromFanfictionDOM(aDOM);

            StorageManager.AddOrRefreshBooksDefinition(_Books);
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
