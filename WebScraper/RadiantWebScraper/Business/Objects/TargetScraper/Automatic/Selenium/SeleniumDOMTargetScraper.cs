using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Radiant.Common.Diagnostics;
using Radiant.WebScraper.RadiantWebScraper.Configuration;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;
using Radiant.WebScraper.RadiantWebScraper.Scrapers;

namespace Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Automatic.Selenium
{
    public class SeleniumDOMTargetScraper : IScraperTarget
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems)
        {
            WebScrapersConfiguration _ScrapersConfiguration = WebScraperConfigurationManager.ReloadConfig();

            IWebDriver _Driver = null;
            switch (aSupportedBrowser)
            {
                case Browser.Firefox:

                    var _FirefoxOptions = new FirefoxOptions
                    {
                        BrowserExecutableLocation = _ScrapersConfiguration.GetBrowserConfigurationBySupportedBrowser(aSupportedBrowser)?.ExecutablePath
                    };
                    _FirefoxOptions.AddArguments("--headless");
                    _FirefoxOptions.SetPreference("javascript.enabled", true);
                    _Driver = new FirefoxDriver(_FirefoxOptions) { Url = aUrl };
                    break;
                case Browser.Chrome:
                    var _ChromeOptions = new ChromeOptions();
                    _ChromeOptions.AddArguments("--headless");
                    _Driver = new ChromeDriver(_ChromeOptions) { Url = aUrl };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aSupportedBrowser), aSupportedBrowser, $"Browser [{aSupportedBrowser}] is unsupported.");
            }

            try
            {
                this.DOM = _Driver.PageSource;
                _Driver.Dispose();
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("84edd8dd-2ead-40d4-a731-0d91d26e125b", $"Couldn't fetch [{aUrl}] using [{aSupportedBrowser}].", _Ex);
                _Driver?.Dispose();
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string DOM { get; set; }
    }
}
