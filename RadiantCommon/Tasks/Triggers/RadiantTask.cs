using System;
using System.Collections.Generic;

namespace Radiant.Common.Tasks.Triggers
{
    public abstract class RadiantTask : IRadiantTask
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected abstract void TriggerNowImplementation();

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void EvaluateTriggers()
        {
            if (!this.IsEnabled)
                return;

            foreach (ITrigger _Trigger in this.Triggers)
            {
                bool _TriggerNow = _Trigger.Evaluate();

                if (_TriggerNow)
                {
                    ForceTriggerNow();
                    return;
                }
            }
        }

        public void ForceTriggerNow()
        {
            if (!this.IsEnabled)
                return;

            TriggerNowImplementation();
            this.LastDateTimeTriggered = DateTime.Now;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool IsEnabled { get; set; }
        public List<ITrigger> Triggers { get; set; }
        public DateTime LastDateTimeTriggered { get; set; }
    }
}
