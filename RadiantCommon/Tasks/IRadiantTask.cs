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
        bool IsEnabled { get; set; }
    }
}
