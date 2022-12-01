using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Radiant.Common.Tasks.Triggers
{
    public abstract class RadiantTask : IRadiantTask
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        /// <summary>
        /// A working task is a task that is currently evaluating or triggering
        /// </summary>
        private bool fIsWorking;

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected abstract void TriggerNowImplementation();

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void EvaluateTriggers()
        {
            if (!this.IsEnabled || fIsWorking)
                return;

            fIsWorking = true;
            try
            {
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
            finally
            {
                fIsWorking = false;

                lock (TaskLockObject)
                {
                    State = TaskState.Idle;
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

        [XmlIgnore]
        [JsonIgnore]
        public DateTime LastDateTimeTriggered { get; set; }
        public TaskState State { get; set; }
        public List<ITrigger> Triggers { get; set; }
        public string UID { get; set; } = Guid.NewGuid().ToString("D");
        public object TaskLockObject { get; set; } = new object();
    }
}
