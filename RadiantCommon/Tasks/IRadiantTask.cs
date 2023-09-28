using System;
using Radiant.Common.Tasks.Triggers;

namespace Radiant.Common.Tasks
{
    public interface IRadiantTask : ITriggerDependent
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public delegate bool ValidationBeforeTriggerDelegate();

        void EvaluateTriggers(ValidationBeforeTriggerDelegate onValidateBeforeTrigger, Action onTriggered);
        void ForceTriggerNow(ValidationBeforeTriggerDelegate onValidateBeforeTrigger, Action onTriggered);

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool IsEnabled { get; set; }
        public bool IsForegroundExclusive { get; set; }// When a task runs, does it needs exclusivity over foreground process ex: manual web scraper
        public TaskState State { get; set; }
        public object TaskLockObject { get; set; }
        public string UID { get; set; }
    }
}
