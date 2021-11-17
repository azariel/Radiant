using System.Drawing;

namespace EveFight.Configuration
{
    public class EveFightConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool CompactUI { get; set; }
        public int PlayerNameMaxDigits { get; set; }
        public TankInfo TankInfo { get; set; }
        public ThreatDetermination ThreatDetermination { get; set; }

        public bool UseThreatColorByDPS { get; set; } = true;
        public ThreatColorByDps ThreatColorByDps { get; set; } = new();

        public Point UILocation { get; set; }

        /// <summary>
        /// Is UI moveable, alterable
        /// </summary>
        public bool UILocked { get; set; }

        public Size UISize { get; set; }
    }
}
