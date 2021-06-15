using System;

namespace Radiant.Common.Tasks.Triggers
{
    public class ScheduleTrigger : ITrigger
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ScheduleTrigger()
        {
            if (this.ResetTriggeredTimesOnStart)
                AcknowledgeHasTriggered();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void AcknowledgeHasTriggered()
        {
            DateTime _Now = DateTime.Now;
            this.LastTriggeredDateTime = _Now;
            this.NextDateTimeToTrigger = _Now.AddSeconds(this.TriggerEverySeconds);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public bool Evaluate()
        {
            DateTime _Now = DateTime.Now;
            if (_Now < this.NextDateTimeToTrigger)
                return false;

            AcknowledgeHasTriggered();
            return true;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DateTime LastTriggeredDateTime { get; set; }
        public DateTime NextDateTimeToTrigger { get; set; }

        /// <summary>
        /// Discard triggered times when program start.
        /// </summary>
        public bool ResetTriggeredTimesOnStart { get; set; }

        public long TriggerEverySeconds { get; set; }
    }
}
