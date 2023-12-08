namespace EveBee.Scenarios
{
    public enum Scenario
    {
        Idle,// Default. Not knowing what to do. The point is to idling for a time
        Repairing,
        Undocking,
        StoppingShip,
        ActivatingForeverModules,
        Docking,
        InWarpToCombatSite,
        FindingNextCombatSite,
        DeterminingValidityOfCurrentCombatSite,
        GoingBackToStationGrid,
        PreparingCombat,
        CombatFirstWave,// First manually the first ~10 enemies (get rid of scraming frigs for instance)
        SemiIdleCombat// From time to time, select the first target manually and focus it. Beside that, just be idle until the combat site is done
    }
}
