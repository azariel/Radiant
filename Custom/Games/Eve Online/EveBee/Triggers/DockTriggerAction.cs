using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;

namespace EveBee.Triggers
{
    public class DockTriggerAction : ITriggerAction
    {
        public void Trigger()
        {
            BeeState.MustFlee = true;
        }
    }
}
