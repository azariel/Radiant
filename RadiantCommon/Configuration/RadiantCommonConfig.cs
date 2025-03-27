using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static Radiant.Common.Diagnostics.LoggingManager;

namespace Radiant.Common.Configuration
{
    /// <summary>
    /// Global configuration for Radiant
    /// </summary>
    public class RadiantCommonConfig
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public RadiantTasksConfig Tasks { get; set; } = new();

        [JsonConverter(typeof(StringEnumConverter))]
        public LogVerbosity LogVerbosity { get; set; } = LogVerbosity.Minimal;
    }
}
