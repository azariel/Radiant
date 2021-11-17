using System;
using EveFight.UIElements;

namespace EveFight.Models
{
    public class Ship
    {
        public int DPS { get; set; }

        /// <summary>
        /// Last time model was updated
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// UID
        /// </summary>
        public string PlayerName { get; set; }

        public string ShipName { get; set; }

        // Properties
        public ThreatLevel ThreatLevel { get; set; }

        public ThreatType ThreatType { get; set; }
    }
}
