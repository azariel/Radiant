﻿using System.Collections.Generic;
using Radiant.Custom.Games.EveOnline.EveFight.UIElements;

namespace Radiant.Custom.Games.EveOnline.EveFight.Models
{
    public class ShipDefinition
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string ShipType { get; set; }
        public ThreatType ThreatType { get; set; }
        public int SignatureRadius { get; set; }
        public List<Weakness> Weaknesses { get; set; }
        public DamageProfileWeakness DamageProfileWeakness { get; set; }
    }
}
