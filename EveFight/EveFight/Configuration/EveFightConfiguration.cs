using System.Drawing;

namespace EveFight.Configuration
{
    public class EveFightConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool CompactUI { get; set; }
        public string LogsDirectoryPath { get; set; }

        /// <summary>
        /// If user is running multiple instances, he can decide which account to track by player name in log
        /// </summary>
        public string ForcePlayerNameInLogs { get; set; }
        public int PlayerNameMaxDigits { get; set; }
        public TankInfo TankInfo { get; set; }
        public ThreatColorByDps ThreatColorByDps { get; set; } = new();
        public ThreatDetermination ThreatDetermination { get; set; }

        public Point UILocation { get; set; }

        /// <summary>
        /// Is UI moveable, alterable
        /// </summary>
        public bool UILocked { get; set; }

        public Size UISize { get; set; }

        public bool UseThreatColorByDPS { get; set; } = true;
    }
}
