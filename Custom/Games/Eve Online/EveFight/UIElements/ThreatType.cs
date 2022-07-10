using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EveFight.UIElements;

[JsonConverter(typeof(StringEnumConverter))]
public enum ThreatType
{
    LOGI,
    TACKLE,
    DPS,
    COMMAND,
    FREIGHTER,
    MINING,
    ECM,
    TANK
    // Stealth Bomber ?
}
