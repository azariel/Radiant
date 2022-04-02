using System.Diagnostics;
using Radiant.Common.Tasks.Triggers;

namespace Radiant.Common.Tasks
{
    public class ShutdownComputerTask : RadiantTask
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // Just straight up shutdown by using CMD
            Process.Start("shutdown", "/s /t 5");
        }
    }
}
