using Radiant.Common.Diagnostics;
using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;

namespace EveBee.Triggers
{
    public class InvalidCombatSiteTriggerAction : ITriggerAction
    {
        public void Trigger()
        {
            LoggingManager.LogToFile("a2e5cbb8-5804-4b50-b138-7cf8b0ea4a23", "Current combat site is invalid.");
            BeeState.CurrentCombatSiteIsInvalid = true;
        }
    }
}
