using System;

namespace Radiant.Common.Tasks.Triggers
{
    public class ScheduleTrigger : RadiantTrigger, ITrigger
    {
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

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public bool Evaluate()
        {
            DateTime _Now = DateTime.Now;
            if (_Now < fNextDateTimeToTrigger)
                return false;

            AcknowledgeHasTriggered();
            return true;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Discard triggered times when program start.
        /// </summary>

        //public bool ResetTriggeredTimesOnStart { get; set; }

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
