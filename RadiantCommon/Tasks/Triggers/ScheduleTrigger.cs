using System;

namespace Radiant.Common.Tasks.Triggers
{
    public class ScheduleTrigger : RadiantTrigger, ITrigger
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

            return aNow.TimeOfDay > this.BlackOutTimeFrame.BlackOutStart && aNow.TimeOfDay < this.BlackOutTimeFrame.BlackOutEnd;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public bool Evaluate()
        {
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
