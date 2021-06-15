using System;
using RadiantInputsManager.ExecutionResults;
using RadiantInputsManager.InputsParam;
using RadiantInputsManager.Linux.xdotool;

namespace RadiantInputsManager
{
    public static class InputsManager
    {
        // ********************************************************************
        //                            Nested Types
        // ********************************************************************
        public enum OperatingSystem
        {
            Linux
        }

        public enum InputType
        {
            Mouse,
            Keyboard
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static IInputExecutionResult ExecuteInput(OperatingSystem aOperatingSystem, InputType aInputType, IInputParam aInputParam)
        {
            return aOperatingSystem switch
            {
                OperatingSystem.Linux => XdoToolInputsManager.Execute(aInputType, aInputParam),
                _ => throw new ArgumentOutOfRangeException(nameof(aOperatingSystem), aOperatingSystem, null)
            };
        }
    }
}
