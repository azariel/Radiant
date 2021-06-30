﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

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
            if (!this.IsEnabled || this.IsWorking)
                return;

            this.IsWorking = true;
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
            } finally
            {
                this.IsWorking = false;
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
        public string UID { get; set; } = Guid.NewGuid().ToString("D");

        /// <summary>
        /// A working task is a task that is currently evaluating or triggering
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsWorking { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public DateTime LastDateTimeTriggered { get; set; }
        public List<ITrigger> Triggers { get; set; }
    }
}
