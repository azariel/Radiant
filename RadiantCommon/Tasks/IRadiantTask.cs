using Radiant.Common.Tasks.Triggers;

namespace Radiant.Common.Tasks
{
    public interface IRadiantTask : ITriggerDependent
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        void EvaluateTriggers();
        void ForceTriggerNow();

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool IsEnabled { get; set; }
        public string UID { get; set; }
    }
}
