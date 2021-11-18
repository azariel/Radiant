using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using EveFight.Configuration;
using EveFight.Models;
using EveFight.UIElements;

namespace EveFight.Managers
{
    internal static class ShipsManager
    {
        // Nb lines already processed from logs
        private static int fNbLinesToSkip;

        static ShipsManager()
        {
            string[] _Lines = GetEveFileLogInfo(out FileSystemInfo? _EveLogFile);

            if (_EveLogFile != null)
                fNbLinesToSkip = _Lines.Length;
        }

        private static void ProcessNewDPSUpdateLine(string aEveLogLine)
        {
            string[] _SplittedLineOnBold = aEveLogLine.Split("</b>");

            if (_SplittedLineOnBold.Length < 3)
                throw new Exception("Incorrect value parsing from Eve logs.");

            // Damage
            string[] _SplittedLineOnDMG = _SplittedLineOnBold[0].Split("<b>");

            if (_SplittedLineOnDMG.Length < 2)
                throw new Exception("Incorrect damage value parsing from Eve logs.");

            string _DMG = _SplittedLineOnDMG.Last().Trim();
            if (!int.TryParse(_DMG, out int _Damage))
                throw new Exception($"Incorrect damage value parsing from Eve logs. [{_DMG}] is not an int.");

            // Attacker Name
            string[] _SplittedLineOnAttackerName = _SplittedLineOnBold[1].Split("<b>");

            if (_SplittedLineOnAttackerName.Length < 2)
                throw new Exception("Incorrect attacker name value parsing from Eve logs.");

            string[] _SplittedOnLowerThan = _SplittedLineOnAttackerName.Last().Split(">");

            if (_SplittedOnLowerThan.Length < 2)
                throw new Exception("Incorrect attacker name color value parsing from Eve logs.");

            // Contains Alliance and ship type name as well
            string _AttackerCompleteName = _SplittedOnLowerThan.Last().Trim();

            string[] _SplittedOnBracket = _AttackerCompleteName.Split("[");

            // Note that if target is an NPC, we don't have the alliance and ship type name
            //if (_SplittedOnBracket.Length < 2)
            //    throw new Exception("Incorrect attacker sub name value parsing from Eve logs.");

            string _AttackerName = _SplittedOnBracket.First().Trim();

            var _Ship = ShipList.SingleOrDefault(s => s.PlayerName.Equals(_AttackerName));

            // TODO: Update ship

            if (_Ship == null)
            {
                string[] _SplittedOnParenthesis = _SplittedLineOnAttackerName.Last().Split("(");

                string _AttackerShipName = null;
                if (_SplittedOnParenthesis.Length >= 2)
                    _AttackerShipName = _SplittedOnParenthesis.Last().Substring(startIndex: 0, _SplittedOnParenthesis.Last().Length - 1).Trim();

                // Create new attacker
                Ship _NewAttacker = new()
                {
                    PlayerName = _AttackerName,
                    LastUpdate = DateTime.Now,
                    ShipName = _AttackerShipName,
                    ThreatType = ThreatType.DPS// TODO
                };

                _NewAttacker.AddNewDamageOutput(_Damage);
                ShipList.Add(_NewAttacker);
                return;
            }

            _Ship.AddNewDamageOutput(_Damage);
        }

        private static void ProcessNewUpdateLines(string[] aEveLogFileLines)
        {
            //string _SimpleRegexBetweenTags = "<.*?b.*?>(.*)<.*?/b.*?>";
            string _SimpleRegexWarpScramble = "warp.*attempt";

            foreach (string _EveLogLine in aEveLogFileLines)
            {
                string _LowerInvariantLine = _EveLogLine.ToLowerInvariant();
                Match _ShipIsScrambler = Regex.Match(_LowerInvariantLine, _SimpleRegexWarpScramble);

                if (_ShipIsScrambler.Success)
                {
                    // TODO: ship is tackler !
                    continue;
                }

                // TODO: get weapon type to know if they're sniping ship, or have bad tracking, etc

                // DPS log line
                // -------------------------------
                ProcessNewDPSUpdateLine(_EveLogLine);
            }
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
            } else
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

        /// <summary>
        /// Fetch update from eve stats file logs and update local model
        /// </summary>
        public static void UpdateShips()
        {
            // Update from disk
            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();
            string[] _EveLogFileLines = GetEveFileLogInfo(out FileSystemInfo? _EveLogFile);

            if (_EveLogFile == null)
                throw new Exception($"Log File for player [{_Config.ForcePlayerNameInLogs}] was not found.");

            int _NbTotalLines = _EveLogFileLines.Length;
            if (_EveLogFileLines.Length < fNbLinesToSkip)
                fNbLinesToSkip = 0;

            // Skip already processed lines
            _EveLogFileLines = _EveLogFileLines.Skip(fNbLinesToSkip).ToArray();

            ProcessUpdateUserLines(_EveLogFileLines.Where(w =>
                    w.Contains("(combat)") &&
                    w.Contains(">to<") &&
                    !w.Contains("misses")).ToArray());

            // Skip non-combat lines and player's lines
            var _AttackersLines = _EveLogFileLines.Where(w =>
                    w.Contains("(combat)") &&
                    w.Contains(">from<") &&
                    !w.Contains("misses")).ToArray();

            ProcessNewUpdateLines(_AttackersLines);

            UserShip.UpdateDPS();
            foreach (Ship _Ship in ShipList)
                _Ship.UpdateDPS();

            // Remove old attackers that aren't on grid anymore
            ShipList.RemoveAll(a => (DateTime.Now - a.LastUpdate).TotalSeconds > 30);

            // Next loop, skip the lines we just did
            fNbLinesToSkip = _NbTotalLines;
        }

        private static void ProcessUpdateUserLines(string[] aEveLogFileLines)
        {
            foreach (string _EveLogLine in aEveLogFileLines) 
            {
                string[] _SplittedLineOnBold = _EveLogLine.Split("</b>");

                if (_SplittedLineOnBold.Length < 3)
                    throw new Exception("Incorrect value parsing from Eve logs.");

                // Damage
                string[] _SplittedLineOnDMG = _SplittedLineOnBold[0].Split("<b>");

                if (_SplittedLineOnDMG.Length < 2)
                    throw new Exception("Incorrect damage value parsing from Eve logs.");

                string _DMG = _SplittedLineOnDMG.Last().Trim();
                if (!int.TryParse(_DMG, out int _Damage))
                    throw new Exception($"Incorrect damage value parsing from Eve logs. [{_DMG}] is not an int.");

                UserShip.AddNewDamageOutput(_Damage);
            }
        }

        private static string[] GetEveFileLogInfo(out FileSystemInfo? aEveLogFile)
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();

            // Fetch all logs file names
            if (!Directory.Exists(_Config.LogsDirectoryPath))
                throw new Exception($"Directory [{_Config.LogsDirectoryPath}] is invalid.");

            var _DirectoryInfo = new DirectoryInfo(_Config.LogsDirectoryPath);

            IOrderedEnumerable<FileSystemInfo> _EveLogFiles = _DirectoryInfo.GetFileSystemInfos().OrderByDescending(o => o.LastWriteTime);
            aEveLogFile = null;

            string[] _Lines = null;
            foreach (FileSystemInfo _Logfile in _EveLogFiles)
            {
                if (!string.IsNullOrWhiteSpace(_Config.ForcePlayerNameInLogs))
                {
                    string _PlayerName = _Config.ForcePlayerNameInLogs.ToLowerInvariant();

                    // Check if logFile is for specified player
                    // Note that Eve will lock this file when writing
                    Stopwatch _Stopwatch = new Stopwatch();
                    string[] _AllLines;
                    while (true)
                    {
                        try
                        {
                            _AllLines = File.ReadAllLines(_Logfile.FullName);
                            break;
                        } catch (Exception _Ex)
                        {
                            if (_Stopwatch.ElapsedMilliseconds > 10000)
                                throw new Exception("Eve FileLog was inaccessible for 10 seconds. Aborting. " + _Ex.Message);

                            Thread.Sleep(millisecondsTimeout: 50);
                        }
                    }

                    _Lines = _AllLines.ToArray();

                    string[] _HeaderLines = _AllLines.Take(100).ToArray();
                    if (_HeaderLines.All(a => !a.ToLowerInvariant().Contains($"listener: {_PlayerName}")))
                        continue;
                }

                aEveLogFile = _Logfile;
                break;
            }

            return _Lines ?? Array.Empty<string>();
        }

        public static List<Ship> ShipList { get; } = new();
        public static Ship UserShip { get; } = new();
    }
}
