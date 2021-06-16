using System;

namespace Radiant.Common.Tasks.Triggers
{
    public class ScheduleTrigger : RadiantTrigger, ITrigger
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ScheduleTrigger()
        {
            //if (this.ResetTriggeredTimesOnStart)
            //    AcknowledgeHasTriggered();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private DateTime fNextDateTimeToTrigger = DateTime.Now;

        private void AcknowledgeHasTriggered()
        {
            DateTime _Now = DateTime.Now;
            fNextDateTimeToTrigger = _Now.AddSeconds(this.TriggerEverySeconds);
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

        public long TriggerEverySeconds { get; set; }
    }
}
