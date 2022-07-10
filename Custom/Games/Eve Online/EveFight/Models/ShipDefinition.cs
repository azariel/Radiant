using System.Collections.Generic;
using EveFight.UIElements;

namespace EveFight.Models
{
    public class ShipDefinition
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string ShipType { get; set; }
        public ThreatType ThreatType { get; set; }
        public List<Weakness> Weaknesses { get; set; }
        public DamageProfileWeakness DamageProfileWeakness { get; set; }
    }
}
