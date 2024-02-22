using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.Utils;
using Radiant.WebScraper.RadiantWebScraper.Configuration;
using Radiant.WebScraper.RadiantWebScraper.Helpers;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;
using Radiant.WebScraper.RadiantWebScraper.Scrapers;

namespace Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Manual
{
    /// <summary>
    /// Base implementation with several helping scrap functions
    /// </summary>
    public abstract class ManualBaseTargetScraper : IScraperTarget
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum TargetScraperCoreOptions
        {
            Screenshot
        }

        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ManualBaseTargetScraper() { }

        public ManualBaseTargetScraper(TargetScraperCoreOptions aOptions)
        {
            fOptions = aOptions;
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private readonly TargetScraperCoreOptions? fOptions;

        private void TryTakeScreenshotAndInfo(string aOutPutPath)
        {
            var _Config = WebScraperConfigurationManager.ReloadConfig();

            if (!_Config.TakeLandingPageScreenshots)
                return;

            DateTime _Now = DateTime.Now;
            ImageUtils.TakeScreenshot(aOutPutPath, out string _);

            // Add a info file beside
            File.WriteAllText(Path.Combine(aOutPutPath, $"{_Now:yyyy-MM-dd HH.mm.ss.fff}-BASE_INFO.txt"), $"Url: {fUrl}");
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected bool fAllowManualOperations;
        protected Browser? fBrowser;
        protected string fUrl;

        protected void WaitForBrowserInputsReadyOrMax(int aMinMsToWait, int aMaxMsToWait = 60000)
        {
            BrowserHelper.WaitForBrowserInputsReadyOrMax(aMinMsToWait, fBrowser, aMaxMsToWait);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public virtual void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems)
        {
            Thread.Sleep(5000);

            fBrowser = aSupportedBrowser;
            fUrl = aUrl;
            fAllowManualOperations = aAllowManualOperations;

            if (!fOptions.HasValue)
                return;

            if (fOptions.Value.HasFlag(TargetScraperCoreOptions.Screenshot))
            {
                string _RootFolder = "Screenshots";

                if (!string.IsNullOrWhiteSpace(fUrl))
                    _RootFolder = Path.Combine(_RootFolder, RegexUtils.GetWebSiteDomain(fUrl));

                // Add current date to root folder
                _RootFolder = Path.Combine(_RootFolder, $"{DateTime.Now:yyyy-MM-dd}");

                TryTakeScreenshotAndInfo(_RootFolder);
                Thread.Sleep(500);
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public byte[] Screenshot { get; set; }
    }
}
