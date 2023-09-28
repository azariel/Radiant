using System.Collections.Generic;
using System.Diagnostics;

namespace Radiant.Common.Tasks.Triggers.Base
{
    /// <summary>
    /// Trigger when specified processes are not currently running.
    /// Useful to avoid running tasks when the user is playing a specific game or is in VPN/Working etc
    /// </summary>
    public class ProcessesNotRunningTrigger : InactivityTrigger, IRadiantTrigger
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private bool AreProcessesNotRunning()
        {
            foreach (string _ProcessName in ProcessesNameNotRunningCollection)
            {
                if (IsProcessRunning(_ProcessName))
                    return false;
            }

            return true;
        }

        private bool IsProcessRunning(string processName)
        {
            var _Processes = Process.GetProcessesByName(processName);
            return _Processes.Length > 0;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override bool Evaluate()
        {
            bool _TriggerState = base.Evaluate();

            if (!_TriggerState)
                return false;

            if (ProcessesNameNotRunningCollection == null || ProcessesNameNotRunningCollection.Count <= 0)
                return false;

            // Trigger only if processes are NOT currently running
            return AreProcessesNotRunning();
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// List of processes name that will avoid the task to runs
        /// </summary>
        public List<string> ProcessesNameNotRunningCollection { get; set; } = new();
    }
}
