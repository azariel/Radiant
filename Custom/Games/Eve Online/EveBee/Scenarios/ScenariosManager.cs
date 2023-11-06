using EveBee.Actions;
using EveBee.Configuration;
using Radiant.Common.Diagnostics;

namespace EveBee.Scenarios
{
    public static class ScenariosManager
    {
        private static Scenario fCurrentScenario = Scenario.Idle;

        public static void Tick()
        {
            var _Config = EveBeeConfigurationManager.ReloadConfig();

            // Check if there are enemies in local
            bool _EnnemiesInLocal = ActionsExecutor.IsThereEnemiesInLocal();

            if (_EnnemiesInLocal)
            {
                // If there are, do nothing. Wait up.
                BeeState.ForceWaitInDockedIdleUntilDateTime = DateTime.Now.AddMinutes(10);// force wait idly in docked for 10 min
            }

            // Fleeing
            if (BeeState.MustFlee)
            {
                LoggingManager.LogToFile("7199df12-3361-444d-b79c-4c4089722056", "Bee must flee. Docking procedures triggered.");

                BeeState.MustFlee = false;

                if (!BeeState.Docked)
                {
                    fCurrentScenario = Scenario.Docking;
                    ActionsExecutor.DockToStation();
                }

                fCurrentScenario = Scenario.Idle;
                return;
            }

            // Going back to station
            if (BeeState.MustGoBackToStationGrid)
            {
                LoggingManager.LogToFile("6f314ee3-f2a8-4c3a-8afd-2a5cf3a8095d", "Bee must go back to station grid.");

                BeeState.MustGoBackToStationGrid = false;

                if (!BeeState.Docked)
                {
                    fCurrentScenario = Scenario.GoingBackToStationGrid;
                    ActionsExecutor.WarpToStation();
                }

                fCurrentScenario = Scenario.Idle;
                return;
            }

            switch (fCurrentScenario)
            {
                case Scenario.Idle:
                    GetScenarioToDoFromIdleState();
                    break;
                case Scenario.FindingNextCombatSite:
                    FindNextCombatSite();
                    break;
                case Scenario.InWarpToCombatSite:

                    AwarenessInCombat();// Since we're not sure if the ship is REALLY in warp or not, check for carrier even if we're technically still in warp

                    if (BeeState.MustFlee)
                        return;

                    if (BeeState.IsWarping)
                    {
                        var _NbMsElapsed = (DateTime.Now - BeeState.WarpStartDateTime).TotalMilliseconds;
                        if (_NbMsElapsed < _Config.WarpMaxTimeToWaitInMs)
                        {
                            LoggingManager.LogToFile("4e23e454-da6e-4c23-9c07-6800f29d9f0c", $"Waiting for Warp to end. Ms left: [{(_Config.WarpMaxTimeToWaitInMs - _NbMsElapsed)}]...");
                            return;// waiting for warp
                        }

                        LoggingManager.LogToFile("64c74357-a5a3-4ea7-b8a6-50b669b504aa", "Warp to combat site ended.");
                        BeeState.IsWarping = false;
                        fCurrentScenario = Scenario.DeterminingValidityOfCurrentCombatSite;
                    }
                    break;
                case Scenario.DeterminingValidityOfCurrentCombatSite:
                    AwarenessInCombat();

                    if (BeeState.MustFlee)
                        return;

                    DetermineValidityOfCurrentCombatSite();
                    break;
                case Scenario.PreparingCombat:

                    AwarenessInCombat();

                    if (BeeState.MustFlee)
                        return;

                    PrepareToCombat();
                    break;
                case Scenario.CombatFirstWave:
                    // TODO: target smallest ship. kill it. Loop for the next few min
                    AttackFirstWaveTargets();
                    break;
                default:
                    return;
            }
        }

        private static void AttackFirstWaveTargets()
        {
            ActionsExecutor.AttackFirstWaveTargets();
        }

        private static void FindNextCombatSite()
        {
            ActionsExecutor.FindNextCombatSite();

            if (BeeState.MustFlee)
                return;

            ActionsExecutor.WarpToNextCombatSite();
            BeeState.IsWarping = true;
            BeeState.WarpStartDateTime = DateTime.Now;
            fCurrentScenario = Scenario.InWarpToCombatSite;
        }

        private static void DetermineValidityOfCurrentCombatSite()
        {
            LoggingManager.LogToFile("2ae84613-43b7-4057-9f3c-ba43d028c9ff", "Validating combat site...");
            bool _CurrentCombatSiteIsInvalid = ActionsExecutor.DetermineValidityOfCurrentCombatSite();

            if (!_CurrentCombatSiteIsInvalid)
            {
                // Combat site is valid, prepare to combat (ex: anchor + afterburner)
                LoggingManager.LogToFile("e3696007-32ea-45a0-9541-322992a35448", "Combat site is valid.");
                fCurrentScenario = Scenario.PreparingCombat;
                return;
            }

            LoggingManager.LogToFile("1190e24e-942d-4821-be43-83c8d6be3a55", "Combat site is invalid.");

            // Remove current site from radar
            BeeState.MustCleanTopMostCombatSite = true;

            // Go back to dock
            BeeState.MustGoBackToStationGrid = true;
        }

        private static void PrepareToCombat()
        {
            LoggingManager.LogToFile("3ac40420-9f77-4ea9-95e2-5f3855eaf00a", "Preparing Bee for combat site (Anchoring, AB, etc)...");
            ActionsExecutor.PrepareToCombat();
            fCurrentScenario = Scenario.CombatFirstWave;
            LoggingManager.LogToFile("3ac40420-9f77-4ea9-95e2-5f3855eaf00a", "Bee is ready.");
        }

        private static void AwarenessInCombat()
        {
            // We already check Local for red every tick
            // Check for Carrier on grid
            bool _MustFlee = ActionsExecutor.IsThereACarrierOnGrid();
            LoggingManager.LogToFile("d9878a9f-2783-4173-ae5b-25453aa65f2f", $"Carrier on grid: [{_MustFlee}]");

            if (_MustFlee)
            {
                // don't forget to ignore the top site
                BeeState.MustCleanTopMostCombatSite = true;
            }
        }

        // Try to find something to do as we're in idle state
        private static void GetScenarioToDoFromIdleState()
        {
            if (BeeState.ForceWaitInDockedIdleUntilDateTime != null)
            {
                if (BeeState.ForceWaitInDockedIdleUntilDateTime > DateTime.Now)
                {
                    return;// continue waiting idly
                }

                LoggingManager.LogToFile("94e3e393-9c2d-4c30-b98d-91e12c939a6c", "Bee idle state is over.");

                // wait is over
                BeeState.ForceWaitInDockedIdleUntilDateTime = null;
            }

            LoggingManager.LogToFile("77fc9fba-55f1-4dc7-a5ae-f3696ece2529", "Bee is idle. Generating next event...");

            // if we're still docked
            if (BeeState.Docked)
            {
                fCurrentScenario = Scenario.Repairing;

                // 1st thing to do is repairing
                ActionsExecutor.RepairShips();

                // 2st thing to do is undock
                fCurrentScenario = Scenario.Undocking;
                LoggingManager.LogToFile("beacb87f-6a24-48f9-b61d-87194826abb6", "Bee is undocking.");
                ActionsExecutor.Undock();
                BeeState.Docked = false;

                fCurrentScenario = Scenario.StoppingShip;
                ActionsExecutor.StopShip();

                fCurrentScenario = Scenario.ActivatingForeverModules;
                LoggingManager.LogToFile("0b39f718-65dd-426c-a3f8-9774bd8fcddc", "Bee is activating forever modules.");
                ActionsExecutor.ActivateForeverModules();

                fCurrentScenario = Scenario.Idle;
            }

            // Ship is currently undocked
            // Clean up scanner to search for an anomaly
            fCurrentScenario = Scenario.FindingNextCombatSite;
        }
    }
}
