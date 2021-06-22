using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Radiant.Common.Helpers;
using Radiant.WebScraper.Helpers;

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
        protected SupportedBrowser? fBrowser;

        protected void WaitForBrowserInputsReadyOrMax(int aMinMsToWait, int aMaxMsToWait = 60000)
        {
            Stopwatch _Stopwatch = new Stopwatch();
            _Stopwatch.Start();

            // TODO: check processes and wait for input
            if (fBrowser.HasValue)
            {
                BrowserHelper.WaitForWebPageToFinishLoadingByBrowser(fBrowser.Value, (int)(aMaxMsToWait - _Stopwatch.ElapsedMilliseconds));
                Thread.Sleep(50);

                if (_Stopwatch.ElapsedMilliseconds > aMaxMsToWait)
                    return;
            }

            int _MinMsToWait = (int)(aMinMsToWait - _Stopwatch.ElapsedMilliseconds);

            if (_MinMsToWait > 0)
                Thread.Sleep(_MinMsToWait);
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private readonly TargetScraperCoreOptions? fOptions;

        private void TakeScreenshot()
        {
            using var _Bitmap = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);
            using Graphics _Graphics = Graphics.FromImage(_Bitmap);

            _Graphics.CopyFromScreen(0, 0, 0, 0, _Bitmap.Size, CopyPixelOperation.SourceCopy);
            _Bitmap.Save("C:\\temp\\a.png");
            this.Screenshot = ImageHelper.ImageToByte2(_Bitmap);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public virtual void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations)
        {
            fBrowser = aSupportedBrowser;

            if (!fOptions.HasValue)
                return;

            if (fOptions.Value.HasFlag(TargetScraperCoreOptions.Screenshot))
            {
                TakeScreenshot();
                Thread.Sleep(500);
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public byte[] Screenshot { get; set; }
    }
}
