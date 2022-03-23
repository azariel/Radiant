﻿using System.Collections.Generic;
using Radiant.Common.Tasks.Triggers;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Automatic.Selenium;
using Radiant.WebScraper.Scrapers.Automatic.Selenium;
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

            //ManualScraper _ManualScraper = new();
            //ProductTargetScraper _ProductScraper = new(ManualBaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            //ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            //List<IScraperItemParser> _ManualScrapers = _Config.ManualScraperSequenceItems.Select(s => (IScraperItemParser)s).ToList();
            //List<DOMParserItem> _DomParsers = _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList();

            //_ManualScraper.GetTargetValueFromUrl(Browser.Firefox, aProductDefinition.Url, _ProductScraper, _ManualScrapers, _DomParsers);

            SeleniumScraper _Scraper = new SeleniumScraper();
            SeleniumDOMTargetScraper _DOMTargetScraper = new SeleniumDOMTargetScraper();

            foreach (RadiantReaderHostModel _Host in _HostBooks)
            {
                _Scraper.GetTargetValueFromUrl(Browser.Firefox, _Host.HostLandingPage, _DOMTargetScraper, null, null);
                ParseBooksFromDOMLandingPage(_DOMTargetScraper.DOM);
            }
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
