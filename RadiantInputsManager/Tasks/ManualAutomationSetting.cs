using RadiantInputsManager.InputsParam;

namespace RadiantInputsManager.Tasks
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
