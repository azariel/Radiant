using Radiant.Common.Diagnostics;
using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;

namespace EveBee.Triggers
{
    public class DockAndWaitTriggerAction : ITriggerAction
    {
        public void Trigger()
        {
            LoggingManager.LogToFile("e81a0ce9-9516-46b1-94fe-e185c3bf951d", "Bee must Dock and wait for 5 min.");
            BeeState.MustFlee = true;

            BeeState.ForceWaitInDockedIdleUntilDateTime = DateTime.Now.AddMinutes(5);// force wait idly in docked for 5 min
        }
    }
}
