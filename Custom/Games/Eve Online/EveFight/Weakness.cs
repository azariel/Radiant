using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EveFight
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Weakness
    {
        // Ships that you can de-fang easily
        Drones,

    }
}
