using EveBee.Scenarios;

namespace EveBee.Configuration
{
    public class EveBeeConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************

        public double WarpMaxTimeToWaitInMs { get; set; } = 60000;
        public RepairShipsScenario RepairShipScenario { get; set; }
        public UndockScenario UndockScenario { get; set; }
        public StopShipScenario StopShipScenario { get; set; }
        public ActivateForeverModulesScenario ActivateForeverModulesScenario { get; set; }
        public DockToStationScenario DockToStationScenario { get; set; }
        public EnemiesInLocalDetectionScenario EnemiesInLocalDetectionScenario { get; set; }
        public CarrierDetectionScenario CarrierDetectionScenario { get; set; }
        public FindNextCombatSiteScenario FindNextCombatSiteScenario { get; set; }
        public WarpToNextCombatSiteScenario WarpToNextCombatSiteScenario { get; set; }
        public DetermineValidityOfCurrentCombatSiteScenario DetermineValidityOfCurrentCombatSiteScenario { get; set; }
        public WarpToStationScenario WarpToStationScenario { get; set; }
        public PrepareToCombatSiteScenario PrepareToCombatSiteScenario { get; set; }
        public AttackFirstWaveTargetsScenario AttackFirstWaveTargetsScenario { get; set; }
    }
}
