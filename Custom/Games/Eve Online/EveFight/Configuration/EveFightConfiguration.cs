using System.Collections.Generic;
using System.Drawing;
using Radiant.Custom.Games.EveOnline.EveFight.Models;

namespace Radiant.Custom.Games.EveOnline.EveFight.Configuration
{
    public class EveFightConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// The compactUI was made to allow more targets to show on the UI in less space
        /// </summary>
        public bool CompactUI { get; set; }

        public int CompactUIContainerHeight { get; set; }

        /// <summary>
        /// GameLogs directory
        /// </summary>
        public string LogsDirectoryPath { get; set; }

        /// <summary>
        /// If user is running multiple instances, he can decide which account to track by specifying a player name in log
        /// </summary>
        public string TrackPlayerNameInLogs { get; set; }
        public ReplayLogInfo ReplaySpecificLogForDebug {get;set;}
        public int PlayerNameMaxDigitsOnUI { get; set; }
        public TankInfo TankInfo { get; set; }
        public ThreatColorByDps ThreatColorByDps { get; set; } = new();
        public ThreatDetermination ThreatDetermination { get; set; }
        public List<ShipDefinition> ShipDefinitions { get; set; } = new();
        public List<WeaponDefinition> WeaponDefinitions { get; set; } = new();

        public Point UILocation { get; set; }

        /// <summary>
        /// Is UI moveable, alterable
        /// </summary>
        public bool UILocked { get; set; }

        // Transparent (and click thought) background
        public bool Transparent { get; set; }

        public Size UISize { get; set; }

        public bool UseThreatColorByDPS { get; set; } = true;

        /// <summary>
        /// cycle, in ms, of dps cycle. Values older than that won't be considered. Ex: consider dps of ships for last 30000 ms.
        /// </summary>
        public int DpsCycleMs { get; set; } = 30000;
    }
}
