using Newtonsoft.Json;
using Radiant.Common.Diagnostics;
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
        private bool fIsWorking = false;

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        /// <summary>
        /// Should the task try to cleanly stop.
        /// IE: The task executed a satisfying loop or is running for so long that it should re-evaluate its trigger.
        /// </summary>
        protected bool fShouldStop = false;

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

            LoggingManager.LogToFile("05efbf2d-3e0b-4127-afb2-02324b30dcb6", $"Task [{UID}] - [{GetType().FullName}] triggered.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            onTriggered?.Invoke();
            this.LastDateTimeTriggered = DateTime.Now;
            TriggerNowImplementation();

            LoggingManager.LogToFile("c26c0d88-1a4e-4b3c-aca0-ce2a5f2c675f", $"Task [{UID}] - [{GetType().FullName}] is done.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
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
