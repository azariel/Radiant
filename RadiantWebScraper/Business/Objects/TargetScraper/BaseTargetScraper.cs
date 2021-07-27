using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.Helpers;
using Radiant.WebScraper.Helpers;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;

namespace Radiant.WebScraper.Business.Objects.TargetScraper
{
    /// <summary>
    /// Base implementation with several helping scrap functions
    /// </summary>
    public abstract class BaseTargetScraper : IScraperTarget
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
        public BaseTargetScraper() { }

        public BaseTargetScraper(TargetScraperCoreOptions aOptions)
        {
            fOptions = aOptions;
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected bool fAllowManualOperations;
        protected SupportedBrowser? fBrowser;
        protected string fUrl;

        protected void WaitForBrowserInputsReadyOrMax(int aMinMsToWait, int aMaxMsToWait = 60000)
        {
            BrowserHelper.WaitForBrowserInputsReadyOrMax(aMinMsToWait, fBrowser, aMaxMsToWait);
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private readonly TargetScraperCoreOptions? fOptions;

        private void TryTakeScreenshot(string aOutPutPath)
        {
            try
            {
                using var _Bitmap = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);
                using Graphics _Graphics = Graphics.FromImage(_Bitmap);

                _Graphics.CopyFromScreen(0, 0, 0, 0, _Bitmap.Size, CopyPixelOperation.SourceCopy);

                DateTime _Now = DateTime.Now;
                string _ImagePath = $"{_Now:yyyy-MM-dd HH.mm.ss.fff}.png";

                if (!Directory.Exists(aOutPutPath))
                    Directory.CreateDirectory(aOutPutPath);

                _Bitmap.Save(Path.Combine(aOutPutPath, _ImagePath));//"C:\\temp\\a.png"));
                this.Screenshot = ImageHelper.ImageToByte2(_Bitmap);

                // Add a info file beside
                File.WriteAllText(Path.Combine(aOutPutPath, $"{_Now:yyyy-MM-dd HH.mm.ss.fff}-INFO.txt"), $"Url: {fUrl}");

            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("1D33CEE8-20F0-4627-9EFE-B2FCFC4E71CE", "Couldn't take screenshot. Operation will be ignored.", _Exception);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public virtual void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<ManualScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems)
        {
            Thread.Sleep(5000);

            fBrowser = aSupportedBrowser;
            fUrl = aUrl;
            fAllowManualOperations = aAllowManualOperations;

            if (!fOptions.HasValue)
                return;

            if (fOptions.Value.HasFlag(TargetScraperCoreOptions.Screenshot))
            {
                TryTakeScreenshot("Screenshots");
                Thread.Sleep(500);
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public byte[] Screenshot { get; set; }
    }
}
