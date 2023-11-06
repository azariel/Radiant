using EveBee.Configuration;
using Newtonsoft.Json;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.Common.Screen.Watcher.PixelsInZone;
using Radiant.Common.Serialization;
using Radiant.InputsManager;
using Radiant.InputsManager.InputsParam;

namespace EveBee.Actions
{
    public static class ActionsExecutor
    {
        public static bool IsThereEnemiesInLocal()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            PixelsInZoneEvaluator.EvaluateZones(_Config.EnemiesInLocalDetectionScenario.ZonesWatcher, null);

            return BeeState.MustFlee;
        }

        public static void RepairShips()
        {
            // TODO: Focus game instance first
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            if (!_Config.RepairShipScenario.Enabled)
            {
                return;
            }

            LoggingManager.LogToFile("56e884b9-1af0-42cb-8453-134e1595d15a", "Bee is repairing.");

            // Select items to repair
            foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.RepairShipScenario.RepairShipsWhenDocked)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
                Thread.Sleep(new Random().Next(250, 550));// Wait for window to open/animation
            }

            Thread.Sleep(new Random().Next(350, 575));
        }

        public static void Undock()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Config.UndockScenario.Undock);
            Thread.Sleep(_Config.UndockScenario.NbMsToWaitForUndock + new Random().Next(250, 550));// random: for anti-bot detection randomness yada yad
        }

        public static void StopShip()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Config.StopShipScenario.StopShip);
            Thread.Sleep(_Config.StopShipScenario.NbMsToWaitForStopping);
        }

        /// <summary>
        /// Enable modules that runs forever, such as multispectrum, etc. AKA the modules that are enabled only ONCE, when we get out of dock
        /// </summary>
        public static void ActivateForeverModules()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.ActivateForeverModulesScenario.ForeverModulesActivation)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
                Thread.Sleep(new Random().Next(650, 1100));// Wait for window to open/animation
            }

            Thread.Sleep(_Config.StopShipScenario.NbMsToWaitForStopping);
        }

        public static void DockToStation()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            // Recall drones
            foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.DockToStationScenario.RecallDrones)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
            }

            // deactivate modules such as afterburner to align quicker. Note there is an issue here. If we just arrived on a combat site and there's a Carrier, we're ACTIVATING the afterburner...buuut the ship isn't "on grid" yet atm, so it's a fake issue
            foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.DockToStationScenario.DeactivateModules)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
            }

            // Align to station
            foreach (MouseActionInputParam _Action in _Config.DockToStationScenario.AlignToStation)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _Action);
            }

            // TODO Check in logFiles for drones return instead ?
            // Wait for drones
            int _NbSeconds = (int)Math.Floor((double)_Config.DockToStationScenario.WaitForDroneDelayMs / 1000);
            int _RemainingMs = (int)Math.Min(_Config.DockToStationScenario.WaitForDroneDelayMs - (_NbSeconds * 1000), 1000);// Math.max as failsafe
            var _AlignAction = _Config.DockToStationScenario.AlignToStation.LastOrDefault();
            for (int i = 0; i < _NbSeconds; i++)
            {
                Thread.Sleep(new Random().Next(950, 1050));

                // Align to station
                if (_AlignAction != null)
                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _AlignAction);
            }

            Thread.Sleep(_RemainingMs);

            for (int i = 0; i < 12; i++)// for ~ 1 min, spam that dock button
            {
                foreach (MouseActionInputParam _Action in _Config.DockToStationScenario.DockToStation)
                {
                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _Action);
                }
                Thread.Sleep(new Random().Next(4500, 6500));
            }

            // Validate that we are docked in Logfiles
            BeeState.Docked = true;
            BeeState.MustFlee = false;
            BeeState.MustGoBackToStationGrid = false;
            BeeState.IsWarping = false;
        }

        public static void FindNextCombatSite()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            LoggingManager.LogToFile("92a917ed-eca7-4427-a0b9-dba26bee2663", $"Finding next combat site...");

            // If for instance the top most site contains a Carrier
            if (BeeState.MustCleanTopMostCombatSite)
            {
                IgnoreAnomaliesFromRadar(1);
                BeeState.MustCleanTopMostCombatSite = false;
            }

            foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.FindNextCombatSiteScenario.GetRadarData)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
                Thread.Sleep(new Random().Next(300, 500));// Wait for clipboard
            }

            // Get Radar data from clipboard
            string _RawRadarData = ClipboardManager.GetClipboardValue();
            List<string> _RadarDataCollection = _RawRadarData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            int _NbAnomaliesToCleanFromRadarUI = 0;
            foreach (string _RadarLineData in _RadarDataCollection)
            {
                if (!_RadarLineData.ToLowerInvariant().Contains(_Config.FindNextCombatSiteScenario.AnomalyNameEnforcerLower))
                {
                    ++_NbAnomaliesToCleanFromRadarUI;
                } else
                {
                    break;// we stop ignoring it as soon as we get one
                }
            }

            LoggingManager.LogToFile("e0c833c5-6275-4ca5-9219-b1b30647ff99", $"Ignoring top [{_NbAnomaliesToCleanFromRadarUI}] of [{_RadarDataCollection.Count}] combat sites...");

            // Cleanup Radar UI (Ignore anomalies that aren't the one we want the Bee to farm)
            IgnoreAnomaliesFromRadar(_NbAnomaliesToCleanFromRadarUI);

            if (_RadarDataCollection.Count - _NbAnomaliesToCleanFromRadarUI <= 0)
            {
                LoggingManager.LogToFile("4abb779d-317c-4da0-b8b3-06026b645b44", $"There is no valid combat site. Docking procedures triggered.");

                // No combat site. Wait 5 min while docked and retry after
                BeeState.ForceWaitInDockedIdleUntilDateTime = DateTime.Now.AddMinutes(5);
                BeeState.MustFlee = true;
            }
        }

        private static void IgnoreAnomaliesFromRadar(int nbAnomaliesToIgnore)
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            for (int i = 0; i < nbAnomaliesToIgnore; i++)
            {
                foreach (var _Action in _Config.FindNextCombatSiteScenario.CleanTopOneRadarResult)
                {
                    // Add noise
                    MouseActionInputParam _TempAction = JsonCommonSerializer.DeserializeFromString<MouseActionInputParam>(JsonCommonSerializer.SerializeToString(_Action));
                    _TempAction.X += new Random().Next(-20, 10);
                    _TempAction.Y += new Random().Next(-2, 2);

                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _TempAction);
                    Thread.Sleep(new Random().Next(100, 550));// wait between actions
                }

                if (IsThereEnemiesInLocal())
                {
                    return;
                }

                Thread.Sleep(new Random().Next(350, 850));// wait between ignore
            }
        }

        public static bool IsThereACarrierOnGrid()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            PixelsInZoneEvaluator.EvaluateZones(_Config.CarrierDetectionScenario.ZonesWatcher, null);

            return BeeState.MustFlee;
        }

        public static void WarpToNextCombatSite()
        {
            LoggingManager.LogToFile("fd86a04f-16fe-4324-b3ee-4e3730b8c163", "Warping to combat site...");

            var _Config = EveBeeConfigurationManager.ReloadConfig();

            foreach (MouseActionInputParam _Action in _Config.WarpToNextCombatSiteScenario.WarpToTopOneRadarResult)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _Action);
            }
        }

        /// <summary>
        /// Afterburner & Anchoring
        /// </summary>
        public static void PrepareToCombat()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.PrepareToCombatSiteScenario.ActivateModules)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
            }

            if (IsThereEnemiesInLocal() || IsThereACarrierOnGrid())
            {
                return;
            }

            foreach (MouseActionInputParam _Action in _Config.PrepareToCombatSiteScenario.Anchor)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _Action);
            }
        }

        public static bool DetermineValidityOfCurrentCombatSite()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            // TODO: validate for Friendly on Grid. If so, ignore this combat site

            PixelsInZoneEvaluator.EvaluateZones(_Config.DetermineValidityOfCurrentCombatSiteScenario.ZonesWatcher, null);

            return BeeState.CurrentCombatSiteIsInvalid;
        }

        public static void AttackFirstWaveTargets()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            // Select all targets
            for (int i = 0; i < _Config.AttackFirstWaveTargetsScenario.NbManualTargetsToDo; i++)
            {
                // Click on enemy
                foreach (MouseActionInputParam _Action in _Config.AttackFirstWaveTargetsScenario.SelectTopTarget)
                {
                    // Add noise
                    MouseActionInputParam _TempAction = JsonCommonSerializer.DeserializeFromString<MouseActionInputParam>(JsonCommonSerializer.SerializeToString(_Action));
                    _TempAction.X += new Random().Next(-2, 2);
                    _TempAction.Y += i * _Config.AttackFirstWaveTargetsScenario.PixelsSpacingBetweenTargetsInOverview;

                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _TempAction);
                }

                // Click on TARGET
                foreach (MouseActionInputParam _Action in _Config.AttackFirstWaveTargetsScenario.TargetSelectedEnemy)
                {
                    MouseActionInputParam _TempAction = JsonCommonSerializer.DeserializeFromString<MouseActionInputParam>(JsonCommonSerializer.SerializeToString(_Action));
                    _TempAction.X += new Random().Next(-2, 2);
                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _TempAction);
                }

                if (IsThereEnemiesInLocal() || IsThereACarrierOnGrid())
                {
                    return;
                }

                // F (attack)
                foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.AttackFirstWaveTargetsScenario.AttackCurrentTarget)
                {
                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
                }
            }

            BeeState.NextManualTargetToFocusDateTime = DateTime.Now.AddSeconds(new Random().Next(60, 180));
            BeeState.NextCombatSiteCompletionValidatorDateTime = DateTime.Now.AddMinutes(5);

            // Attack
            for (int i = 0; i < _Config.AttackFirstWaveTargetsScenario.ContinuousAttackDurationInLoop; i++)
            {
                // F
                foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.AttackFirstWaveTargetsScenario.AttackCurrentTarget)
                {
                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
                }

                Thread.Sleep(new Random().Next(5000, 8500));
            }
        }

        private static bool SafeWait(double waitTimeInMs)
        {
            int _NbSeconds = (int)Math.Floor(waitTimeInMs / 1000);
            int _RemainingMs = (int)Math.Min(waitTimeInMs - (_NbSeconds * 1000), 1000);// Math.max as failsafe
            for (int i = 0; i < _NbSeconds; i++)
            {
                Thread.Sleep(1000);

                if (IsThereEnemiesInLocal() || IsThereACarrierOnGrid())
                {
                    return false;
                }
            }

            Thread.Sleep(_RemainingMs);
            return true;
        }

        /// <summary>
        /// Watch out, we're disregarding drones recall
        /// </summary>
        public static void WarpToStation()
        {
            LoggingManager.LogToFile("196f8777-d122-43ad-8538-7c97650b8a8f", "Warping to station grid...");

            BeeState.IsWarping = true;

            var _Config = EveBeeConfigurationManager.ReloadConfig();

            int _NbSeconds = (int)Math.Floor((double)_Config.WarpToStationScenario.WarpMaxTimeToWaitInMs / 1000);
            int _RemainingMs = (int)Math.Min(_Config.WarpToStationScenario.WarpMaxTimeToWaitInMs - (_NbSeconds * 1000), 1000);// Math.max as failsafe

            foreach (MouseActionInputParam _Action in _Config.WarpToStationScenario.WarpToStation)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _Action);
            }

            // Re-execute last action a few time
            var _LastMouseAction = _Config.WarpToStationScenario.WarpToStation.LastOrDefault();
            if (_LastMouseAction != null)
            {
                for (int i = 0; i < Math.Min(5, _NbSeconds); i++)
                {
                    // Add noise
                    MouseActionInputParam _TempAction = JsonCommonSerializer.DeserializeFromString<MouseActionInputParam>(JsonCommonSerializer.SerializeToString(_LastMouseAction));
                    _TempAction.X += new Random().Next(-2, 2);
                    _TempAction.Y += new Random().Next(-2, 2);
                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _TempAction);
                    Thread.Sleep(new Random().Next(900, 1100));
                }
            }

            int _NbMsRemaining = (Math.Max(0, _NbSeconds - 5) * 1000) + _RemainingMs;
            LoggingManager.LogToFile("4e06dc89-5573-4499-a6fa-1bef52a4d39b", $"In warp to station, waiting [{_NbMsRemaining}] ms...");
            Thread.Sleep(_NbMsRemaining);

            BeeState.IsWarping = false;
        }

        public static void SemiIdleCombat()
        {
            if (DateTime.Now < BeeState.NextManualTargetToFocusDateTime)
            {
                return;
            }

            // Every ~5 min, select a target and focus it
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            foreach (MouseActionInputParam _Action in _Config.AttackFirstWaveTargetsScenario.SelectTopTarget)
            {
                // Add noise
                MouseActionInputParam _TempAction = JsonCommonSerializer.DeserializeFromString<MouseActionInputParam>(JsonCommonSerializer.SerializeToString(_Action));
                _TempAction.X += new Random().Next(-5, 10);
                _TempAction.Y += new Random().Next(-1, 1);
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _TempAction);
            }

            // Click on TARGET
            foreach (MouseActionInputParam _Action in _Config.AttackFirstWaveTargetsScenario.TargetSelectedEnemy)
            {
                MouseActionInputParam _TempAction = JsonCommonSerializer.DeserializeFromString<MouseActionInputParam>(JsonCommonSerializer.SerializeToString(_Action));
                _TempAction.X += new Random().Next(-2, 2);
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, _TempAction);
            }

            if (IsThereEnemiesInLocal() || IsThereACarrierOnGrid())
            {
                return;
            }

            if (!SafeWait(_Config.AttackFirstWaveTargetsScenario.DelayBetweenDroneAttackNewTarget * 2))
            {
                return;
            }

            foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.AttackFirstWaveTargetsScenario.AttackCurrentTarget)
            {
                InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
            }

            BeeState.NextManualTargetToFocusDateTime = DateTime.Now.AddSeconds(new Random().Next(300, 480));
        }

        public static bool DetermineIfCombatSiteIsCompleted()
        {
            // Check every 5 min if the combat site is completed
            if (DateTime.Now < BeeState.NextCombatSiteCompletionValidatorDateTime)
            {
                return false;
            }

            BeeState.NextCombatSiteCompletionValidatorDateTime = DateTime.Now.AddMinutes(5);

            var _Config = EveBeeConfigurationManager.ReloadConfig();

            // TODO: validate for Friendly on Grid. If so, ignore this combat site

            DateTime _BeforeEval = DateTime.Now;
            PixelsInZoneEvaluator.EvaluateZones(_Config.DetermineIfCombatSiteIsCompletedScenario.ZonesWatcher, null);

            if (BeeState.CombatSiteIsOverDateTimeTrigger != null && _BeforeEval < BeeState.CombatSiteIsOverDateTimeTrigger)
            {
                BeeState.CombatSiteIsOverDateTimeTrigger = null;

                if (BeeState.CombatSiteValidatorIterator > 3)
                {
                    BeeState.CombatSiteValidatorIterator = 0;

                    // Recall drones
                    foreach (KeyboardKeyStrokeActionInputParam _Action in _Config.DockToStationScenario.RecallDrones)
                    {
                        InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, _Action);
                    }

                    // Wait for drones to come back. Large amount of time to be sure we're not loosing drones needlessly
                    SafeWait(30000);

                    WarpToStation();
                    return true;

                } else
                {
                    // Make sure. Spawn may take a few secs
                    BeeState.NextCombatSiteCompletionValidatorDateTime = DateTime.Now.AddSeconds(5);
                    ++BeeState.CombatSiteValidatorIterator;
                }
            }

            return false;
        }
    }
}
