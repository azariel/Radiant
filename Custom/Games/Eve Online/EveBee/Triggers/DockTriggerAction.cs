using Radiant.Common.Diagnostics;
using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;

namespace EveBee.Triggers
{
    public class DockTriggerAction : ITriggerAction
    {
        public void Trigger()
        {
            LoggingManager.LogToFile("c4758d9e-bb58-44de-86e1-314c000273bf", "Bee must Dock.");

            BeeState.MustFlee = true;
        }
    }
}
