using System.Collections.Generic;
using EveRay.TriggerActions;
using EveRay.Watch;

namespace EveRay.Zones
{
    public class ZoneWatcher
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool AlwaysShowZone { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public bool SaveImageOnDisk { get; set; }
        public ITriggerAction TriggerAction { get; set; }
        public List<IWatchItem> WatchItems { get; set; }
        public EveRayZone Zone { get; set; } = new();
    }
}
