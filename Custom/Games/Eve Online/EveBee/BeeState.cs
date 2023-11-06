namespace EveBee
{
    public static class BeeState
    {
        public static bool Docked { get; set; } = true;
        public static DateTime? ForceWaitInDockedIdleUntilDateTime { get; set; } = null;
        public static bool MustFlee { get; set; } = false;// Bee must flee, there is danger
        public static bool MustCleanTopMostCombatSite { get; set; } = false;
        public static bool IsWarping { get; set; } = false;
        public static DateTime WarpStartDateTime { get; set; } = DateTime.MinValue;
        public static bool CurrentCombatSiteIsInvalid { get; set; } = false;
        public static bool MustGoBackToStationGrid { get; set; }
        public static DateTime NextManualTargetToFocusDateTime { get; set; } = DateTime.MinValue;
        public static DateTime? CombatSiteIsOverDateTimeTrigger { get; set; } = null;
        public static DateTime NextCombatSiteCompletionValidatorDateTime { get; set; } = DateTime.MinValue;
        public static int CombatSiteValidatorIterator = 0;
    }
}
