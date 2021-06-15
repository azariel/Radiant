using System;
using System.Drawing;
using Radiant.Common.Diagnostics;
using RadiantInputsManager.ExecutionResults;
using RadiantInputsManager.InputsParam;
using RadiantInputsManager.Linux.xdotool;

namespace RadiantInputsManager
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
            IInputExecutionResult _Result = InputsManager.ExecuteInput(InputsManager.InputType.Mouse, new MouseReadInputParam());

            if (!_Result.Success)
                LoggingManager.LogToFile("Couldn't get mouse location.");

            if (_Result is not IInputMouseReadExecutionResult _MouseReadExecutionResult)
            {
                LoggingManager.LogToFile($"Mouse Read execution result wasn't of type {nameof(IInputMouseReadExecutionResult)}.");
                throw new Exception($"Mouse Read execution result wasn't of type {nameof(IInputMouseReadExecutionResult)}.");
            }

            return new Point(_MouseReadExecutionResult.X, _MouseReadExecutionResult.Y);
        }

        public static void KeyboardExecute(Keycode[] aKeycodes, int? aDelay = null)
        {
            IInputExecutionResult _Result = InputsManager.ExecuteInput(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = aDelay,
                KeyStrokeCodes = aKeycodes
            });

            if (!_Result.Success)
                LoggingManager.LogToFile($"Failed to execute keyboard keycodes [{aKeycodes}].");
        }

        public static void KeyboardType(string aTextToType, int aMinimumDelayBetweenKeyStrokes, int aMaximumDelayBetweenKeyStrokes)
        {
            Random _Random = new(DateTime.Now.Millisecond);
            int _Delay = _Random.Next(aMinimumDelayBetweenKeyStrokes, aMaximumDelayBetweenKeyStrokes);

            IInputExecutionResult _Result = InputsManager.ExecuteInput(InputsManager.InputType.Keyboard, new KeyboardTypeActionInputParam
            {
                Delay = _Delay,
                ValueToType = aTextToType
            });

            if (!_Result.Success)
                LoggingManager.LogToFile($"Failed to execute keyboard type [{aTextToType}].");
        }

        public static void MouseClick(Point aPoint, MouseOptions.MouseButtons aButton)
        {
            MoveCursorToLocation(aPoint);
            MouseClickOnCurrentLocation(aButton);
        }

        public static void MouseClickOnCurrentLocation(MouseOptions.MouseButtons aButton)
        {
            IInputExecutionResult _Result = InputsManager.ExecuteInput(InputsManager.InputType.Mouse, new MouseActionInputParam
            {
                Button = aButton
            });

            if (!_Result.Success)
                LoggingManager.LogToFile("Couldn't get mouse location.");
        }

        public static void MoveCursorToLocation(Point aPoint)
        {
            IInputExecutionResult _Result = InputsManager.ExecuteInput(InputsManager.InputType.Mouse, new MouseActionInputParam
            {
                X = aPoint.X,
                Y = aPoint.Y
            });

            if (!_Result.Success)
                LoggingManager.LogToFile("Couldn't get mouse location.");
        }
    }
}
