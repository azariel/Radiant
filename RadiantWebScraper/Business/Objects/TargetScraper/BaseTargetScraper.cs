using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Radiant.Common.Helpers;

namespace Radiant.WebScraper.Business.Objects.TargetScraper
{
    /// <summary>
    /// Base implementation with several helping scrap functions
    /// </summary>
    public class BaseTargetScraper : IScraperTarget
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
        public void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl)
        {
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
