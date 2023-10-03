using System;
using System.Runtime.InteropServices;

namespace Radiant.Common.Tasks.Triggers.Base
{
    /// <summary>
    /// Trigger around the user (computer) inactivity.
    /// I.E: If a user is considered "inactive", we assume that the computer is available to process task.
    /// </summary>
    public class InactivityTrigger : RadiantTrigger, IRadiantTrigger
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct LastInputInfo
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LastInputInfo lastInputInfo);

        private bool IsUserInactive(TimeSpan userInactivityTimespan)
        {
            var _Info = new LastInputInfo();
            _Info.cbSize = (uint)Marshal.SizeOf(_Info);

            if (!GetLastInputInfo(ref _Info))
                return false;

            var _CurrentInactivityTimeSpan = TimeSpan.FromMilliseconds(Environment.TickCount - _Info.dwTime);
            return _CurrentInactivityTimeSpan > userInactivityTimespan;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public virtual bool Evaluate()
        {
            if (!InactivityTriggerEnabled)
                return true;

            // Trigger only if user is inactive
            return IsUserInactive(MinimumInactivityTimeSpan);
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Trigger will trigger only if user is inactive for more than this timeSpan
        /// </summary>
        public TimeSpan MinimumInactivityTimeSpan { get; set; } = TimeSpan.FromMinutes(0);

        public bool InactivityTriggerEnabled { get; set; } = false;
    }
}
