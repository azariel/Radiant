﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Games.EveOnline.EveFight.Configuration;
using Radiant.Custom.Games.EveOnline.EveFight.Helpers;
using Radiant.Custom.Games.EveOnline.EveFight.Models;
using Radiant.Custom.Games.EveOnline.EveFight.UIElements;

namespace Radiant.Custom.Games.EveOnline.EveFight.Managers
{
    internal static class ShipsManager
    {
        // Nb lines already processed from logs
        private static int fNbLinesToSkip;
        private static DateTime fReplayStart = DateTime.UtcNow;

        static ShipsManager()
        {
            string[] _Lines = GetEveFileLogInfo(out FileSystemInfo? _EveLogFile);

            if (_EveLogFile != null)
                fNbLinesToSkip = _Lines.Length;
        }

        private static void ProcessNewDPSUpdateLine(string aEveLogLine, bool aIsTackle)
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

            // Weapon Type
            var _WeaponTypeMatch = Regex.Match(aEveLogLine, "- (.*) -", RegexOptions.IgnoreCase);

            if(!_WeaponTypeMatch.Success)
                throw new Exception($"Could not find weapon type in line [{aEveLogLine}].");

            string _WeaponType = _WeaponTypeMatch.Groups.Values.Last().Value;
            var _WeaponDefinition = WeaponTypeHelper.GetWeaponDefinitionFromRawStr(_WeaponType);

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
                    LastUpdate = DateTime.UtcNow,
                    ShipName = _AttackerShipName,
                    ThreatType = ThreatType.DPS,// TODO
                    WeaponsDefinition = new List<WeaponDefinition>()
                };

                if(_WeaponDefinition != null)
                    _NewAttacker.WeaponsDefinition.Add( _WeaponDefinition );

                _NewAttacker.AddNewDamageOutput(_Damage);
                ShipList.Add(_NewAttacker);
                return;
            }

            if(!_Ship.WeaponsDefinition.Contains(_WeaponDefinition) && _WeaponDefinition != null)
                _Ship.WeaponsDefinition.Add(_WeaponDefinition);

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
                bool _IsTackle = false;

                if (_ShipIsScrambler.Success)
                {
                    // TODO: ship is tackler !
                    continue;
                }

                // TODO: get weapon type to know if they're sniping ship, or have bad tracking, etc

                // DPS log line
                // -------------------------------
                ProcessNewDPSUpdateLine(_EveLogLine, _IsTackle);
            }
        }

        public static int GetTotalDPS() => ShipList.Sum(s => s.DPS);

        public static void UpdateShipListItems(bool aCompactUI, ListBox aShipsListBox)
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.GetConfigFromMemory();

            IOrderedEnumerable<Ship> _OrderedShips;
            if (ShipList.Count(c => c.ThreatType == ThreatType.TACKLE) < _Config.ThreatDetermination.PrioritizeTackleIfNumberOfTacklingShipsBelowThisNumberAndTankIsGreen && GetTotalDPS() < _Config.TankInfo.TotalDPSYellow)
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
            foreach (Ship _OrderedShip in _OrderedShips)
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
            EveFightConfiguration _Config = EveFightConfigurationManager.GetConfigFromMemory();
            string[] _EveLogFileLines = GetEveFileLogInfo(out FileSystemInfo? _EveLogFile);

            if (_EveLogFile == null)
                throw new Exception($"Log File for player [{_Config.TrackPlayerNameInLogs}] was not found.");

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

            // Remove old attackers that aren't active anymore
            ShipList.RemoveAll(a => (DateTime.UtcNow - a.LastUpdate).TotalMilliseconds > _Config.DpsCycleMs);

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
            EveFightConfiguration _Config = EveFightConfigurationManager.GetConfigFromMemory();

            if (_Config.ReplaySpecificLogForDebug.Enabled)
            {
                return GetEveFileLogInfoForReplayDebug(_Config, out aEveLogFile);
            }

            // Fetch all logs file names
            if (!Directory.Exists(_Config.LogsDirectoryPath))
                throw new Exception($"Directory [{_Config.LogsDirectoryPath}] is invalid.");

            var _DirectoryInfo = new DirectoryInfo(_Config.LogsDirectoryPath);

            IOrderedEnumerable<FileSystemInfo> _EveLogFiles = _DirectoryInfo.GetFileSystemInfos().OrderByDescending(o => o.LastWriteTime);
            aEveLogFile = null;

            string[] _Lines = null;
            foreach (FileSystemInfo _Logfile in _EveLogFiles)
            {
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
                    }
                    catch (Exception _Ex)
                    {
                        if (_Stopwatch.ElapsedMilliseconds > 10000)
                            throw new Exception("Eve FileLog was inaccessible for 10 seconds. Aborting. " + _Ex.Message);

                        Thread.Sleep(millisecondsTimeout: 50);
                    }
                }

                _Lines = _AllLines.ToArray();

                if (!string.IsNullOrWhiteSpace(_Config.TrackPlayerNameInLogs))
                {
                    string _PlayerName = _Config.TrackPlayerNameInLogs.ToLowerInvariant();

                    string[] _HeaderLines = _AllLines.Take(100).ToArray();
                    if (_HeaderLines.All(a => !a.ToLowerInvariant().Contains($"listener: {_PlayerName}")))
                    {
                        _Lines = null;
                        continue;
                    }
                }

                aEveLogFile = _Logfile;
                break;
            }

            return _Lines ?? Array.Empty<string>();
        }

        private static string[] GetEveFileLogInfoForReplayDebug(EveFightConfiguration _Config, out FileSystemInfo? aEveLogFile)
        {
            LoggingManager.LogToFile("05C2ABCE-BA1B-49A5-BDF1-2D677E177D15", $"Tracking player [{_Config.TrackPlayerNameInLogs}] in Replay log [{_Config.ReplaySpecificLogForDebug.LogFilePath}].");

            // Fetch all logs file names
            if (!File.Exists(_Config.ReplaySpecificLogForDebug.LogFilePath))
                throw new Exception($"Replay log file [{_Config.ReplaySpecificLogForDebug.LogFilePath}] was not found.");

            aEveLogFile = null;

            var _Logfile = new FileInfo(_Config.ReplaySpecificLogForDebug.LogFilePath);

            string[] _Lines = null;

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
                }
                catch (Exception _Ex)
                {
                    if (_Stopwatch.ElapsedMilliseconds > 10000)
                        throw new Exception("Eve FileLog was inaccessible for 10 seconds. Aborting. " + _Ex.Message);

                    Thread.Sleep(millisecondsTimeout: 50);
                }
            }

            _Lines = _AllLines.ToArray();
            if (!string.IsNullOrWhiteSpace(_Config.TrackPlayerNameInLogs))
            {
                string _PlayerName = _Config.TrackPlayerNameInLogs.ToLowerInvariant();

                _Lines = _AllLines.ToArray();

                string[] _HeaderLines = _AllLines.Take(100).ToArray();
                if (_HeaderLines.All(a => !a.ToLowerInvariant().Contains($"listener: {_PlayerName}")))
                    return null;
            }

            aEveLogFile = _Logfile;

            // Filter the lines returned to simulate a replay
            // Time elapsed since the start of the replay
            TimeSpan _TimeElapsedSinceStartOfReplay = DateTime.UtcNow - fReplayStart;
            DateTime? _FirstReplayLineDateTime = null;
            List<string> _LinesToKeepForReplay = new();
            foreach (string _Line in _Lines) 
            {
                DateTime? _LineDateTime = GetDateTimeFromLogLine(_Line);

                if (_LineDateTime == null)
                    continue;// Skip line (most probably a header line or info line)

                if (_LineDateTime != null && _FirstReplayLineDateTime == null)
                    _FirstReplayLineDateTime = _LineDateTime;

                TimeSpan _TimeElapsedFromFirstRelayLineToCurrentLine = _LineDateTime.Value - _FirstReplayLineDateTime.Value;

                if (_TimeElapsedFromFirstRelayLineToCurrentLine >= _TimeElapsedSinceStartOfReplay)
                    _LinesToKeepForReplay.Add(_Line);
            }

            return _LinesToKeepForReplay.ToArray() ?? Array.Empty<string>();
        }

        public static DateTime? GetDateTimeFromLogLine(string aRawLogLine)
        {
            if (string.IsNullOrWhiteSpace(aRawLogLine))
                return null;

            var _Regex = Regex.Match(aRawLogLine, "\\[ (.*) \\]");

            if (!_Regex.Success)
                return null;

            string _DateStr = _Regex.Groups.Values.Last().Value;

            if (!DateTime.TryParse(_DateStr, out DateTime _DateTime))
                return null;

            return _DateTime;
        }

        public static List<Ship> ShipList { get; } = new();
        public static Ship UserShip { get; } = new();
    }
}
