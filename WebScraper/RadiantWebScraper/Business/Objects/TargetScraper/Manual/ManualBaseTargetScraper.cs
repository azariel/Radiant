using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.Utils;
using Radiant.WebScraper.Helpers;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers;

namespace Radiant.WebScraper.Business.Objects.TargetScraper.Manual
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
            try
            {
                using var _Bitmap = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);// TODO: get screen size dynamically
                using Graphics _Graphics = Graphics.FromImage(_Bitmap);

                _Graphics.CopyFromScreen(0, 0, 0, 0, _Bitmap.Size, CopyPixelOperation.SourceCopy);

                DateTime _Now = DateTime.Now;
                string _ImagePath = $"{_Now:yyyy-MM-dd HH.mm.ss.fff}.png";

                if (!Directory.Exists(aOutPutPath))
                    Directory.CreateDirectory(aOutPutPath);

                _Bitmap.Save(Path.Combine(aOutPutPath, _ImagePath));
                this.Screenshot = ImageUtils.ImageToByte2(_Bitmap);

                // Add a info file beside
                File.WriteAllText(Path.Combine(aOutPutPath, $"{_Now:yyyy-MM-dd HH.mm.ss.fff}-INFO.txt"), $"Url: {fUrl}");

            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("1D33CEE8-20F0-4627-9EFE-B2FCFC4E71CE", "Couldn't take screenshot. Operation will be ignored.", _Exception);
            }
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
