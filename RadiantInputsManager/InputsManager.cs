using System;
using System.Threading;
using Radiant.Common.OSDependent;
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
        public enum InputType
        {
            Mouse,
            Keyboard
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static readonly object fIsWorkingLock = new();

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static IInputExecutionResult ExecuteConcurrentInputWithOverrideOfExclusivity(InputType aInputType, IInputParam aInputParam)
        {
            // Simulate light delay to avoid mismatch inputs
            Thread.Sleep(30);

            SupportedOperatingSystem _OperatingSystem = OperatingSystemHelper.GetCurrentOperatingSystem();
            return _OperatingSystem switch
            {
                SupportedOperatingSystem.Linux => XdoToolInputsManager.Execute(aInputType, aInputParam),
                SupportedOperatingSystem.Windows => Win32InputsManager.Execute(aInputType, aInputParam),
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

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public static bool IsWorking { get; private set; }
    }
}
