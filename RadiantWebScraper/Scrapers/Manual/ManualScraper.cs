using System.Diagnostics;

namespace Radiant.WebScraper.Scrapers.Manual
{
    public class ManualScraper : IScraper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************

        public string GetDOMFromUrl(string aUrl)
        {
            Process _Process = new()
            {
                StartInfo =
                {
                    Arguments = $"--start-maximized {aUrl}",
                    FileName = "chrome",
                    CreateNoWindow = true,
                    UseShellExecute = true
                }
            };

            _Process.Start();

            return null;
        }
    }
}
