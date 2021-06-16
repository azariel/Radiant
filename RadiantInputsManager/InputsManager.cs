using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
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
        //                            Private
        // ********************************************************************
        private static object fIsWorkingLock = new();

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static IInputExecutionResult ExecuteConcurrentInputWithOverrideOfExclusivity(InputType aInputType, IInputParam aInputParam)
        {
            // Simulate light delay to avoid mismatch inputs
            Thread.Sleep(30);

            OperatingSystem _OperatingSystem = GetCurrentOperatingSystem();
            return _OperatingSystem switch
            {
                OperatingSystem.Linux => XdoToolInputsManager.Execute(aInputType, aInputParam),
                OperatingSystem.Windows => Win32InputsManager.Execute(aInputType, aInputParam),
                _ => throw new ArgumentOutOfRangeException(nameof(_OperatingSystem), _OperatingSystem, "Operating system unhandled")
            };
        }

        /// <summary>
        /// Execute the action assuring the caller of the system inputs exclusivity for the duration of the action.
        /// </summary>
        /// <param name="aActionToExecuteWithExclusivity"></param>
        public static void ExecuteInputsWithExclusivity(Action aActionToExecuteWithExclusivity)
        {
            if (aActionToExecuteWithExclusivity == null)
                return;

            while (true)
            {
                lock (fIsWorkingLock)
                {
                    if (!IsWorking)
                    {
                        IsWorking = true;
                        break;
                    }
                }
            }

            try
            {
                aActionToExecuteWithExclusivity.Invoke();
            } finally
            {
                lock (fIsWorkingLock)
                {
                    IsWorking = false;
                }
            }
        }

        public static OperatingSystem GetCurrentOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OperatingSystem.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OperatingSystem.Windows;

            throw new ArgumentOutOfRangeException("Current operating system isn't handled. Please report this to the administrator.");
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public static bool IsWorking { get; private set; }
    }
}
