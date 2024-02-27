using System;
using System.Drawing;
using Radiant.Common.Diagnostics;
using Radiant.InputsManager.ExecutionResults;
using Radiant.InputsManager.InputsParam;
using Radiant.InputsManager.Linux.xdotool;

namespace Radiant.InputsManager
{
    public static class ManualAutomation
    {
        // ********************************************************************
        //                            Nested Types
        // ********************************************************************

        public enum ManualAutomationOperationType
        {
            KeyboardExecute,
            KeyboardType,
            MouseClick,
            MoveCursorToLocation
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static Point GetCursorPosition()
        {
            IInputExecutionResult _Result = InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, new MouseReadInputParam());

            if (!_Result.Success)
                LoggingManager.LogToFile("4DFD6C1E-5AA0-4037-9A98-FAB3646AC541", "Couldn't get mouse location.");

            if (_Result is not IInputMouseReadExecutionResult _MouseReadExecutionResult)
            {
                LoggingManager.LogToFile("73B77EFF-D01A-4242-B963-D4E8149E788B", $"Mouse Read execution result wasn't of type {nameof(IInputMouseReadExecutionResult)}.");
                throw new Exception($"Mouse Read execution result wasn't of type {nameof(IInputMouseReadExecutionResult)}.");
            }

            return new Point(_MouseReadExecutionResult.X, _MouseReadExecutionResult.Y);
        }

        public static void KeyboardExecute(Keycode[] aKeycodes, int? aDelay = null)
        {
            IInputExecutionResult _Result = InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = aDelay,
                KeyStrokeCodes = aKeycodes
            });

            if (!_Result.Success)
                LoggingManager.LogToFile("BEFC53F9-D50E-4FCD-824E-A8ED798740CF", $"Failed to execute keyboard keycodes [{aKeycodes}].");
        }

        public static void KeyboardType(string aTextToType, int aMinimumDelayBetweenKeyStrokes, int aMaximumDelayBetweenKeyStrokes)
        {
            Random _Random = new(DateTime.UtcNow.Millisecond);
            int _Delay = _Random.Next(aMinimumDelayBetweenKeyStrokes, aMaximumDelayBetweenKeyStrokes);

            IInputExecutionResult _Result = InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardTypeActionInputParam
            {
                Delay = _Delay,
                ValueToType = aTextToType
            });

            if (!_Result.Success)
                LoggingManager.LogToFile("E6012DDA-BD0C-4AC2-BC13-2F1B8EAE6E9B", $"Failed to execute keyboard type [{aTextToType}].");
        }

        public static void MouseClick(Point aPoint, MouseOptions.MouseButtons aButton)
        {
            MoveCursorToLocation(aPoint);
            MouseClickOnCurrentLocation(aButton);
        }

        public static void MouseClickOnCurrentLocation(MouseOptions.MouseButtons aButton)
        {
            IInputExecutionResult _Result = InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, new MouseActionInputParam
            {
                Button = aButton
            });

            if (!_Result.Success)
                LoggingManager.LogToFile("278D42D9-03B6-43ED-B7CD-7FB595AC8E47", "Couldn't get mouse location.");
        }

        public static void MoveCursorToLocation(Point aPoint)
        {
            IInputExecutionResult _Result = InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Mouse, new MouseActionInputParam
            {
                X = aPoint.X,
                Y = aPoint.Y
            });

            if (!_Result.Success)
                LoggingManager.LogToFile("4E48F6EA-3450-45E7-A75F-2BF73974E9A3","Couldn't get mouse location.");
        }
    }
}
