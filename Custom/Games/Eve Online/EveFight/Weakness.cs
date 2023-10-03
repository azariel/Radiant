using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Radiant.Custom.Games.EveOnline.EveFight
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Weakness
    {
        // Ships that you can de-fang easily
        Drones,

    }
}
