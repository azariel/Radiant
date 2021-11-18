using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EveFight.UIElements;

namespace EveFight.Models
{
    public class Ship
    {
        public class TimedDamage
        {
            public int Damage { get; set; }
            public DateTime Time { get; set; }
        }

        private List<TimedDamage> fTimedDamages = new();

        public void AddNewDamageOutput(int aDamage)
        {
            DateTime _Now = DateTime.Now;

            fTimedDamages.Add(new TimedDamage
            {
                Damage = aDamage,
                Time = _Now
            });

            // Cleanup old data
            fTimedDamages.RemoveAll(r => (_Now - r.Time).TotalSeconds >= 30);
            
            this.LastUpdate = _Now;
        }

        public void UpdateDPS()
        {
            this.DPS = GetDPS();
        }

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

        public int GetDPS()
        {
            if (fTimedDamages.Count <= 0)
                return 0;

            var _MaxTime = DateTime.Now;
            var _MaxRegisteredTime = fTimedDamages.Max(m => m.Time);
            var _MinTime = fTimedDamages.Min(m => m.Time);

            double _ElapsedSeconds = (_MaxTime - _MinTime).TotalMilliseconds / 1000;
            
            double _DamageOverThisPeriod = fTimedDamages.Sum(s => s.Damage);

            if (_ElapsedSeconds < 1)
                return (int)_DamageOverThisPeriod;
            else if (_ElapsedSeconds > 10 && (_MaxTime-_MaxRegisteredTime).TotalMilliseconds < 3000)
                _ElapsedSeconds = (_MaxRegisteredTime - _MinTime).TotalMilliseconds / 1000;

            double _DPS = _DamageOverThisPeriod / _ElapsedSeconds;

            return (int)_DPS;
        }
    }
}
