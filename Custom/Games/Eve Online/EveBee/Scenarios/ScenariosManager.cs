using EveBee.Actions;
using EveBee.Configuration;
using Radiant.Common.Diagnostics;
using Radiant.InputsManager;
using Radiant.InputsManager.InputsParam;
using Radiant.InputsManager.Linux.xdotool;

namespace EveBee.Scenarios
{
    public static class ScenariosManager
    {
        private static Scenario fCurrentScenario = Scenario.Idle;

        public static void Tick()
        {
            var _Config = EveBeeConfigurationManager.GetConfigFromMemory();

            // TODO: If we're scrammed, kill 1 frig and retry
            // TODO: Detect if an enemy player is on grid. If so, click on warp directly, forget about the drones
            // idea Ctr+A in pirates overview to know how many frigs there are

            // If health is low, go Dock and come back
            if (!BeeState.MustFlee)
            {
                ActionsExecutor.DetectIfBeeHealthIsLow();
            }

            // Check if there are enemies in local
            ActionsExecutor.IsThereEnemiesInLocal();

            // Fleeing
            if (BeeState.MustFlee)
            {
                BeeState.MustFlee = false;

                if (!BeeState.Docked)
                {
                    LoggingManager.LogToFile("7199df12-3361-444d-b79c-4c4089722056", "Bee must flee. Docking procedures triggered.");
                    fCurrentScenario = Scenario.Docking;
                    ActionsExecutor.DockToStation();
                }

                fCurrentScenario = Scenario.Idle;

                if (BeeState.Docked)
                {
                    // Don't spam local detection
                    Thread.Sleep(30000);
                }

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
                        var _NbMsElapsed = (DateTime.UtcNow - BeeState.WarpStartDateTime).TotalMilliseconds;
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
                    // target smallest ship. kill it. Loop for the next few min
                    AttackFirstWaveTargets();
                    break;
                case Scenario.SemiIdleCombat:
                    SemiIdleCombat();
                    break;
                default:
                    return;
            }
        }

        private static void SemiIdleCombat()
        {
            ActionsExecutor.SemiIdleCombat();

            // determine if combat site is over
            if (ActionsExecutor.DetermineIfCombatSiteIsCompleted())
                fCurrentScenario = Scenario.Idle;
        }

        private static void AttackFirstWaveTargets()
        {
            ActionsExecutor.AttackFirstWaveTargets();
            fCurrentScenario = Scenario.SemiIdleCombat;
        }

        private static void FindNextCombatSite()
        {
            ActionsExecutor.FindNextCombatSite();

            if (BeeState.MustFlee)
                return;

            ActionsExecutor.WarpToNextCombatSite();
            BeeState.IsWarping = true;
            BeeState.WarpStartDateTime = DateTime.UtcNow;
            fCurrentScenario = Scenario.InWarpToCombatSite;
        }

        private static void DetermineValidityOfCurrentCombatSite()
        {
            LoggingManager.LogToFile("2ae84613-43b7-4057-9f3c-ba43d028c9ff", "Validating combat site...");
            bool _CurrentCombatSiteIsInvalid = ActionsExecutor.DetermineValidityOfCurrentCombatSite();

            Thread.Sleep(new Random().Next(200, 400));

            bool _CurrentCombatSiteIsInvalid2 = ActionsExecutor.DetermineValidityOfCurrentCombatSite();

            if (!_CurrentCombatSiteIsInvalid || !_CurrentCombatSiteIsInvalid2)
            {
                // Combat site is valid, prepare to combat (ex: anchor + afterburner)
                LoggingManager.LogToFile("e3696007-32ea-45a0-9541-322992a35448", "Combat site is valid.");
                fCurrentScenario = Scenario.PreparingCombat;
                return;
            }

            LoggingManager.LogToFile("1190e24e-942d-4821-be43-83c8d6be3a55", "Combat site is invalid.");

            BeeState.CurrentCombatSiteIsInvalid = false;

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
            LoggingManager.LogToFile("a7a373db-4a11-4466-a83c-082fce38b580", $"Bee is idle. Docked=[{BeeState.Docked}]. MustFlee=[{BeeState.MustFlee}]. IsWarping=[{BeeState.IsWarping}]. MustGoBackToStationGrid=[{BeeState.MustGoBackToStationGrid}]. ForceWaitInDockedIdleUntilDateTime=[{BeeState.ForceWaitInDockedIdleUntilDateTime}]. Now: [{DateTime.UtcNow}].");

            // if we're still docked
            if (BeeState.Docked)
            {
                if (BeeState.ForceWaitInDockedIdleUntilDateTime != null)
                {
                    if (BeeState.ForceWaitInDockedIdleUntilDateTime > DateTime.UtcNow)
                    {
                        Thread.Sleep(new Random().Next(30000,60000));// inaccurate wait, don't spam local detection

                        // mouse mouse a bit
                        InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, new MouseActionInputParam
                        {
                            Button = MouseOptions.MouseButtons.None,
                            X = new Random().Next(250,500),
                            Y = new Random().Next(250,500),
                        });

                        return;// continue waiting idly
                    }

                    LoggingManager.LogToFile("94e3e393-9c2d-4c30-b98d-91e12c939a6c", "Bee idle state is over.");

                    // wait is over
                    BeeState.ForceWaitInDockedIdleUntilDateTime = null;
                }

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
