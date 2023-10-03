using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Radiant.InputsManager.InputsParam;

namespace Radiant.WebScraper.RadiantWebScraper.Scrapers.Manual
{
    public class ManualScraperSequenceItemByInput : ManualScraperSequenceItem
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public IInputParam InputParam { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public InputsManager.InputsManager.InputType InputType { get; set; } = InputsManager.InputsManager.InputType.Keyboard;
    }
}
