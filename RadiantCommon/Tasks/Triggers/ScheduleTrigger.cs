using System;
using Radiant.Common.Tasks.Triggers.Base;

namespace Radiant.Common.Tasks.Triggers
{
    /// <summary>
    /// Trigger around a specific time schedule.
    /// </summary>
    public class ScheduleTrigger : ProcessesNotRunningTrigger, IRadiantTrigger
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public class ScheduleTriggerBlackOutTimeFrame
        {
            // ********************************************************************
            //                            Properties
            // ********************************************************************
            public TimeSpan BlackOutEnd { get; set; }
            public TimeSpan BlackOutStart { get; set; }
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private DateTime fNextDateTimeToTrigger;
        private long fTriggerEveryXSeconds;

        private void AcknowledgeHasTriggered()
        {
            DateTime _Now = DateTime.Now;
            fNextDateTimeToTrigger = _Now.AddSeconds(this.TriggerEveryXSeconds);
        }

        private bool IsTriggerBlockedByBlackOut(DateTime aNow)
        {
            if (this.BlackOutTimeFrame == null)
                return false;

            double _BlackOutStartNbSeconds = this.BlackOutTimeFrame.BlackOutStart.TotalSeconds;
            double _BlackOutEndNbSeconds = this.BlackOutTimeFrame.BlackOutEnd.TotalSeconds;
            double _NowTotalSeconds = aNow.TimeOfDay.TotalSeconds;

            // ex: 00h00 to 23h59
            if (_BlackOutStartNbSeconds < _BlackOutEndNbSeconds)
            {
                // If we are in the blackout scheduled zone
                return _BlackOutStartNbSeconds < _NowTotalSeconds && _BlackOutEndNbSeconds > _NowTotalSeconds;
            }
            else if (_BlackOutEndNbSeconds < _BlackOutStartNbSeconds)// ex: 16h00 to 04h00
            {
                return _BlackOutStartNbSeconds < _NowTotalSeconds || _BlackOutEndNbSeconds > _NowTotalSeconds;
            }
            else
            {
                // If Start and End are equal, we are in a perpetual blackout
                return true;
            }

            // temporary
            throw new Exception("ccda1e5d-5cca-471b-9017-535807a5d91d_unhandled");

            // ex: 23h30 to 04h30
            //return _BlackOutStartNbSeconds > _NowTotalSeconds && _BlackOutEndNbSeconds < _NowTotalSeconds;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public bool Evaluate()
        {
            if (!base.Evaluate())
                return false;

            DateTime _Now = DateTime.Now;

            if (IsTriggerBlockedByBlackOut(_Now))
                return false;

            if (_Now < fNextDateTimeToTrigger)
                return false;

            AcknowledgeHasTriggered();
            return true;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Trigger wont trigger in the specified timeFrame, if provided
        /// </summary>
        public ScheduleTriggerBlackOutTimeFrame BlackOutTimeFrame { get; set; }

        /// <summary>
        /// Discard triggered times when program start.
        /// </summary>
        public long TriggerEveryXSeconds
        {
            get => fTriggerEveryXSeconds;
            set
            {
                fTriggerEveryXSeconds = value;

                // Change NextDateTimeToTrigger
                AcknowledgeHasTriggered();
            }
        }
    }
}
