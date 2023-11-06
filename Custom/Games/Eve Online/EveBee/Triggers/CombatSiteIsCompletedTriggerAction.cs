using Radiant.Common.Diagnostics;
using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;

namespace EveBee.Triggers
{
    public class CombatSiteIsCompletedTriggerAction : ITriggerAction
    {
        public void Trigger()
        {
            LoggingManager.LogToFile("48a6385c-28f6-46fa-aa9a-92eb1261c08d", "Combat site is deemed as completed.");
            BeeState.CombatSiteIsOverDateTimeTrigger = DateTime.Now;
        }
    }
}
