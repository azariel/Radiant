using Radiant.InputsManager.InputsParam;

namespace Radiant.InputsManager.Tasks
{
    public class ManualAutomationSetting
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public IInputParam InputParam { get; set; }
        public ManualAutomation.ManualAutomationOperationType ManualAutomationOperationType { get; set; }
    }
}
