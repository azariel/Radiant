using System.Collections.Generic;
using EveRay.Zones;

namespace EveRay.Configuration
{
    public class EveRayConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<ZoneWatcher> ZonesWatcher { get; set; } = new();
    }
}
