using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;

namespace EveBee.Triggers
{
    public class CombatSiteIsCompletedTriggerAction : ITriggerAction
    {
        public void Trigger()
        {
            BeeState.CombatSiteIsStillValidDateTimeTrigger = DateTime.UtcNow;
        }
    }
}
