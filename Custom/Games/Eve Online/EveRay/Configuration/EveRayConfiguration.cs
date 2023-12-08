using System.Collections.Generic;
using Radiant.Common.Screen.Watcher.PixelsInZone.Models;

namespace Radiant.Custom.Games.EveOnline.EveRay.Configuration
{
    public class EveRayConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<PixelsInZoneAreaModel> ZonesWatcher { get; set; } = new();
    }
}
