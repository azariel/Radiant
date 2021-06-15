using System;
using System.Runtime.InteropServices;
using RadiantInputsManager.ExecutionResults;
using RadiantInputsManager.InputsParam;
using RadiantInputsManager.Linux.xdotool;
using RadiantInputsManager.Windows;

namespace RadiantInputsManager
{
    public static class InputsManager
    {
        // ********************************************************************
        //                            Nested Types
        // ********************************************************************
        public enum OperatingSystem
        {
            Linux,
            Windows
        }

        public enum InputType
        {
            Mouse,
            Keyboard
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static IInputExecutionResult ExecuteInput(InputType aInputType, IInputParam aInputParam)
        {
            OperatingSystem _OperatingSystem = GetCurrentOperatingSystem();
            return _OperatingSystem switch
            {
                OperatingSystem.Linux => XdoToolInputsManager.Execute(aInputType, aInputParam),
                OperatingSystem.Windows => Win32InputsManager.Execute(aInputType, aInputParam),
                _ => throw new ArgumentOutOfRangeException(nameof(_OperatingSystem), _OperatingSystem, "Operating system unhandled")
            };
        }

        public static OperatingSystem GetCurrentOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OperatingSystem.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OperatingSystem.Windows;

            throw new ArgumentOutOfRangeException("Current operating system isn't handled. Please report this to the administrator.");
        }
    }
}
