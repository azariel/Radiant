using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.WebScraper.RadiantWebScraper.Configuration
{
    public static class WebScraperConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string WEB_SCRAPER_CONFIG_FILE_NAME = "RadiantWebScrapersConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static WebScrapersConfiguration fWebScraperConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static WebScrapersConfiguration GetConfigFromMemory()
        {
            return fWebScraperConfiguration ?? ReloadConfig();
        }

        public static WebScrapersConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(WEB_SCRAPER_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();
            
            string _ConfigFileContent = File.ReadAllText(WEB_SCRAPER_CONFIG_FILE_NAME);
            fWebScraperConfiguration = JsonCommonSerializer.DeserializeFromString<WebScrapersConfiguration>(_ConfigFileContent);
            return fWebScraperConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fWebScraperConfiguration == null)
                fWebScraperConfiguration = new WebScrapersConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fWebScraperConfiguration);
            File.WriteAllText(WEB_SCRAPER_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(WebScrapersConfiguration aWebScraperConfiguration)
        {
            fWebScraperConfiguration = aWebScraperConfiguration;
        }
    }
}
