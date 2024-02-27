using System;
using System.Collections.Generic;
using System.Linq;
using Radiant.Custom.Games.EveOnline.EveFight.Configuration;
using Radiant.Custom.Games.EveOnline.EveFight.UIElements;

namespace Radiant.Custom.Games.EveOnline.EveFight.Models
{
    public class Ship
    {
        public class TimedDamage
        {
            public int Damage { get; set; }
            public DateTime Time { get; set; }
        }

        private readonly List<TimedDamage> fTimedDamages = new();
        private string fShipName;

        public Ship()
        {
            RefreshShipDefinition();
        }

        private void RefreshShipDefinition()
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.GetConfigFromMemory();

            if (!string.IsNullOrWhiteSpace(this.ShipName))
            {
                string _LowerShipName = this.ShipName.ToLowerInvariant();
                this.Definition = _Config.ShipDefinitions.FirstOrDefault(f => f.ShipType.ToLowerInvariant().Equals(_LowerShipName));
            }
        }

        public void AddNewDamageOutput(int aDamage)
        {
            DateTime _Now = DateTime.UtcNow;

            fTimedDamages.Add(new TimedDamage
            {
                Damage = aDamage,
                Time = _Now
            });

            // Cleanup old data
            fTimedDamages.RemoveAll(r => (_Now - r.Time).TotalMilliseconds >= EveFightConfigurationManager.GetConfigFromMemory().DpsCycleMs);

            this.LastUpdate = _Now;
        }

        public int GetDPS()
        {
            if (fTimedDamages.Count <= 0)
                return 0;

            var _MaxTime = DateTime.UtcNow;
            var _MaxRegisteredTime = fTimedDamages.Max(m => m.Time);
            var _MinTime = fTimedDamages.Min(m => m.Time);

            double _ElapsedSeconds = (_MaxTime - _MinTime).TotalMilliseconds / 1000;

            double _DamageOverThisPeriod = fTimedDamages.Sum(s => s.Damage);

            if (_ElapsedSeconds < 1)
                return 0;// (int)_DamageOverThisPeriod;

            if (_ElapsedSeconds > 10 && (_MaxTime - _MaxRegisteredTime).TotalMilliseconds < 3000)
                _ElapsedSeconds = (_MaxRegisteredTime - _MinTime).TotalMilliseconds / 1000;

            double _DPS = _DamageOverThisPeriod / _ElapsedSeconds;

            return (int)_DPS;
        }

        public void UpdateDPS()
        {
            this.DPS = GetDPS();
        }

        public ShipDefinition Definition { get; set; }

        public int DPS { get; set; }

        /// <summary>
        /// Last time model was updated
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// UID
        /// </summary>
        public string PlayerName { get; set; }

        public string ShipName
        {
            get => fShipName;
            set
            {
                fShipName = value;
                RefreshShipDefinition();
            }
        }

        // Properties
        public ThreatLevel ThreatLevel { get; set; }

        public ThreatType ThreatType { get; set; }

        public List<WeaponDefinition> WeaponsDefinition { get; set; }
    }
}
