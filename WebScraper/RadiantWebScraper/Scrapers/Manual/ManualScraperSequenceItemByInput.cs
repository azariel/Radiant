using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;

namespace Radiant.WebScraper.Scrapers.Manual
{
    public class ManualScraperSequenceItemByInput : ManualScraperSequenceItem
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public IInputParam InputParam { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public InputsManager.InputType InputType { get; set; } = InputsManager.InputType.Keyboard;
    }
}
