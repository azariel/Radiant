﻿using System;
using Radiant.Common.Tasks.Triggers;

namespace Radiant.Common.Tasks
{
    public interface IRadiantTask : ITriggerDependent
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        void EvaluateTriggers(Action onTriggered);
        void ForceTriggerNow(Action onTriggered);

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool IsEnabled { get; set; }
        public TaskState State { get; set; }
        public object TaskLockObject { get; set; }
        public string UID { get; set; }
    }
}
