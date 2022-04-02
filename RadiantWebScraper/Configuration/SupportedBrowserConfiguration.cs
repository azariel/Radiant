namespace Radiant.WebScraper.Configuration
{
    public class SupportedBrowserConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        //public AutonomousDriversConfig AutonomousDriversConfig { get; set; }
        public string ExecutablePath { get; set; }
        public string LinuxCommand { get; set; }
        public int NbMsToWaitOnBrowserStart { get; set; } = 10000;
        public Browser SupportedBrowser { get; set; }
    }
}
