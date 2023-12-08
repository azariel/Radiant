using System.Collections.Generic;
using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;
using Radiant.Common.Screen.Watcher.PixelsInZone.WatchItems;

namespace Radiant.Common.Screen.Watcher.PixelsInZone.Models
{
    public class PixelsInZoneAreaModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool AlwaysShowZoneUI { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public bool SaveImageOnDiskOnTriggered { get; set; }
        public ITriggerAction TriggerAction { get; set; }
        public List<IPixelsInZoneWatchItem> WatchItems { get; set; }
        public PixelsInZoneAreaManager Zone { get; set; } = new();
    }
}
