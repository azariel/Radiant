using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;

namespace EveBee.Triggers
{
    public class InvalidCombatSiteTriggerAction : ITriggerAction
    {
        public void Trigger()
        {
            BeeState.CurrentCombatSiteIsInvalid = true;
        }
    }
}
