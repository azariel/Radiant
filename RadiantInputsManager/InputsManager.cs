﻿using System;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent;
using Radiant.InputsManager.ExecutionResults;
using Radiant.InputsManager.InputsParam;
using Radiant.InputsManager.Linux.xdotool;
using Radiant.InputsManager.Windows;

namespace Radiant.InputsManager
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
            try
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
            catch (Exception ex)
            {
                LoggingManager.LogToFile("755b3557-8c5c-437e-b3da-ef01636204ed", $"Couldn't execute input action [{aInputType}] of Param [{aInputParam}]. Re-throwing.", ex);
                throw;
            }
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
            }
            finally
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
