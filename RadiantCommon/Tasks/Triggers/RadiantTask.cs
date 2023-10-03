using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Radiant.Common.Tasks.Triggers
{
    public abstract class RadiantTask : IRadiantTask
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        /// <summary>
        /// A working task is a task that is currently evaluating or triggering
        ///// </summary>
        private bool fIsWorking;

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        /// <summary>
        /// Should the task try to cleanly stop.
        /// IE: The task executed a satisfying loop or is running for so long that it should re-evaluate its trigger.
        /// </summary>
        protected bool fShouldStop;

        protected abstract void TriggerNowImplementation();

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void EvaluateTriggers(IRadiantTask.ValidationBeforeTriggerDelegate onValidateBeforeTrigger, Action onTriggered)
        {
            if (!this.IsEnabled || fIsWorking)
                return;

            fIsWorking = true;
            try
            {
                foreach (IRadiantTrigger _Trigger in this.Triggers)
                {
                    bool _TriggerNow = _Trigger.Evaluate();

                    if (_TriggerNow)
                    {
                        ForceTriggerNow(onValidateBeforeTrigger, onTriggered);
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

        public void ForceTriggerNow(IRadiantTask.ValidationBeforeTriggerDelegate onValidateBeforeTrigger, Action onTriggered)
        {
            if (!this.IsEnabled)
                return;

            // If the delegate is null, ignore it
            if (onValidateBeforeTrigger != null && !onValidateBeforeTrigger.Invoke())
                return;

            onTriggered?.Invoke();
            TriggerNowImplementation();
            this.LastDateTimeTriggered = DateTime.Now;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool IsEnabled { get; set; }
        public bool IsForegroundExclusive { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public DateTime LastDateTimeTriggered { get; set; }

        public TaskState State { get; set; } = TaskState.Idle;
        public List<IRadiantTrigger> Triggers { get; set; }
        public string UID { get; set; } = Guid.NewGuid().ToString("D");
        public object TaskLockObject { get; set; } = new object();
    }
}
