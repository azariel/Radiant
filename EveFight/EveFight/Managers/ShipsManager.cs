using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using EveFight.Configuration;
using EveFight.Models;
using EveFight.UIElements;

namespace EveFight.Managers
{
    internal static class ShipsManager
    {
        // DEBUG
        static ShipsManager()
        {
            Random _Random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 4; i++)
            {
                Ship _ListItemShip = new()
                {
                    ThreatType = (ThreatType)Math.Min(i, 2),
                    ThreatLevel = (ThreatLevel)i,
                    PlayerName = $"Player{i}_{_Random.Next(0, 1999999999)}"
                };
                ShipList.Add(_ListItemShip);

                Thread.Sleep(150);
            }

            ShipList[0].ShipName = "Scythe";
            ShipList[1].ShipName = "Stilleto";
            ShipList[2].ShipName = "Omen Navy Issue";
            ShipList[3].ShipName = "Caracal Navy Issue";
        }

        public static int GetTotalDPS() => ShipList.Sum(s => s.DPS);

        public static void UpdateShipListItems(bool aCompactUI, ListBox aShipsListBox)
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();

            IOrderedEnumerable<Ship> _OrderedShips;
            if (ShipList.Count(c => c.ThreatType == ThreatType.TACKLE) < _Config.ThreatDetermination.PrioritizeTackleIfNumberOfTacklingShipsBelowThisNumber)
            {
                if (ShipList.Sum(s => s.DPS) < _Config.ThreatDetermination.PrioritizeLogiShipsIfDpsBelowThisNumber)
                    _OrderedShips = ShipList.OrderByDescending(o => o.ThreatType == ThreatType.TACKLE).ThenBy(t => t.ThreatType).ThenByDescending(t => t.DPS);
                else
                    _OrderedShips = ShipList.OrderByDescending(o => o.ThreatType == ThreatType.TACKLE).ThenByDescending(t => t.DPS);
            }
            else
            {
                if (ShipList.Sum(s => s.DPS) < _Config.ThreatDetermination.PrioritizeLogiShipsIfDpsBelowThisNumber)
                    _OrderedShips = ShipList.OrderByDescending(o => o.ThreatType == ThreatType.LOGI).ThenByDescending(t => t.DPS);
                else
                    _OrderedShips = ShipList.OrderByDescending(t => t.DPS);
            }

            aShipsListBox.Items.Clear();
            foreach (var _OrderedShip in _OrderedShips)
            {
                ListItemShip _ListItem = new()
                {
                    Ship = _OrderedShip
                };

                if (aCompactUI)
                    _ListItem.SetCompactUI();

                aShipsListBox.Items.Add(_ListItem);
            }
        }

        //------------

        /// <summary>
        /// Fetch update from eve stats file logs and update local model
        /// </summary>
        public static void UpdateShips()
        {
            // TODO: get update from disk

            // DEBUG
            foreach (Ship _Ship in ShipList)
            {
                Random _Random = new Random(DateTime.Now.Millisecond);

                _Ship.DPS += _Random.Next(-500, 500);

                if (_Ship.DPS < 0)
                    _Ship.DPS = 0;

                _Ship.LastUpdate = DateTime.Now;
                Thread.Sleep(150);
            }

            //--------

            // Remove old players that aren't on grid anymore
            ShipList.RemoveAll(a => (DateTime.Now - a.LastUpdate).TotalSeconds > 30);

            // TODO: update fShipList
        }

        public static List<Ship> ShipList { get; } = new();
    }
}
