using System.Collections.Generic;
using Radiant.Custom.Games.EveOnline.EveRay.Zones;

namespace Radiant.Custom.Games.EveOnline.EveRay.Configuration
{
    public class EveRayConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<ZoneWatcher> ZonesWatcher { get; set; } = new();
    }
}
