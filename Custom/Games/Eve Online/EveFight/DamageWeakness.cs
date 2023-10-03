using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Radiant.Custom.Games.EveOnline.EveFight
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DamageWeakness
    {
        // Damage type weakness
        EM,
        Thermal,
        Explosion,
        Kinetic,
    }
}
