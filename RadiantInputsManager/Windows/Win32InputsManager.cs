using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using Radiant.InputsManager.ExecutionResults;
using Radiant.InputsManager.InputsParam;
using Radiant.InputsManager.Linux.xdotool;

namespace Radiant.InputsManager.Windows
{
    public static class Win32InputsManager
    {
        private static IInputExecutionResult ExecuteKeyboardOperation(IKeyboardInputParam aInputParam)
        {
            switch (aInputParam)
            {
                case IKeyboardKeyStrokeActionInputParam _KeyboardKeyStrokeActionInputParam:
                    ExecuteKeyStrokes(_KeyboardKeyStrokeActionInputParam);
                    break;
                case IKeyboardTypeActionInputParam _KeyboardTypeActionInputParam:
                    ExecuteKeyboardType(_KeyboardTypeActionInputParam);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aInputParam));
            }

            InputKeyboardExecutionResult _InputKeyboardExecutionResult = new()
            {
                Success = true
            };

            return _InputKeyboardExecutionResult;
        }

        private static void ExecuteKeyboardType(IKeyboardTypeActionInputParam aKeyboardTypeActionInputParam)
        {
            Win32Helper.ExecuteKeyboardTextType(aKeyboardTypeActionInputParam.ValueToType, aKeyboardTypeActionInputParam.Delay ?? 30);
        }

        private static void ExecuteKeyStrokes(IKeyboardKeyStrokeActionInputParam aKeyboardKeyStrokeActionInputParam)
        {
            int _MsDelay = Math.Max(50, aKeyboardKeyStrokeActionInputParam.Delay ?? 0);

            // Press all one by one
            foreach (Keycode _KeyCode in aKeyboardKeyStrokeActionInputParam.KeyStrokeCodes)
            {
                Win32Helper.ExecuteKeyboardKey(_KeyCode, Win32Helper.KeyStrokeAction.Press);
                Thread.Sleep(_MsDelay);
            }

            Thread.Sleep(50);

            _MsDelay = Math.Max(100, aKeyboardKeyStrokeActionInputParam.Delay / 10 ?? 0);

            // Release all one by one, but quicker
            foreach (Keycode _KeyCode in aKeyboardKeyStrokeActionInputParam.KeyStrokeCodes.Reverse())
            {
                Win32Helper.ExecuteKeyboardKey(_KeyCode, Win32Helper.KeyStrokeAction.Release);
                Thread.Sleep(_MsDelay);
            }
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static void ExecuteMouseActionOperation(IMouseActionInputParam aMouseActionInputParam)
        {
            if (aMouseActionInputParam.Button != MouseOptions.MouseButtons.None)
                ExecuteMouseClickAction(aMouseActionInputParam);
            else
                ExecuteMouseMoveAction(aMouseActionInputParam);
        }

        private static void ExecuteMouseClickAction(IMouseActionInputParam aMouseActionInputParam)
        {
            //// Move mouse to centerScreen if required
            //if (aMouseActionInputParam.GoBackToCenterScreenFirst)
            //{
            //    var _ScreenSize = Win32Helper.GetScreenSize();
            //    var _CenterScreenMouseActionInputParam = new MouseActionInputParam
            //    {
            //        Button = MouseOptions.MouseButtons.None,
            //        X = _ScreenSize.Width / 2,
            //        Y = _ScreenSize.Height / 2,
            //    };
            //    ExecuteMouseMoveAction(aMouseActionInputParam);
            //}

            // Move mouse to location first
            ExecuteMouseMoveAction(aMouseActionInputParam);

            // Then click
            Win32Helper.ExecuteMouseEvent(aMouseActionInputParam.Button);
        }

        private static void ExecuteMouseMoveAction(IMouseActionInputParam aMouseActionInputParam)
        {
            Point _CursorLocation = Win32Helper.GetCursorPosition();

            while (true)
            {
                int _Magnitude = new Random().Next(5, 15);

                if (aMouseActionInputParam.X > _CursorLocation.X)
                {
                    if (aMouseActionInputParam.X - _CursorLocation.X <= _Magnitude * 5)
                        _CursorLocation.X++;
                    else
                        _CursorLocation.X += _Magnitude;
                } else if (aMouseActionInputParam.X < _CursorLocation.X)
                {
                    if (_CursorLocation.X - aMouseActionInputParam.X <= _Magnitude * 5)
                        _CursorLocation.X--;
                    else
                        _CursorLocation.X -= _Magnitude;
                }

                if (aMouseActionInputParam.Y > _CursorLocation.Y)
                {
                    if (aMouseActionInputParam.Y - _CursorLocation.Y <= _Magnitude * 5)
                        _CursorLocation.Y++;
                    else
                        _CursorLocation.Y += _Magnitude;
                } else if (aMouseActionInputParam.Y < _CursorLocation.Y)
                {
                    if (_CursorLocation.Y - aMouseActionInputParam.Y <= _Magnitude * 5)
                        _CursorLocation.Y--;
                    else
                        _CursorLocation.Y -= _Magnitude;
                }

                Win32Helper.SetCursorPosition(_CursorLocation.X, _CursorLocation.Y);

                if (_CursorLocation.X == aMouseActionInputParam.X && _CursorLocation.Y == aMouseActionInputParam.Y)
                {
                    break;
                }

                Thread.Sleep(10);
            }
        }

        private static IInputExecutionResult ExecuteMouseOperation(IMouseInputParam aInputParam)
        {
            if (aInputParam == null)
                return null;

            switch (aInputParam)
            {
                case IMouseActionInputParam _MouseActionInputParam:
                    ExecuteMouseActionOperation(_MouseActionInputParam);
                    break;
                case IMouseReadInputParam _MouseReadInputParam:
                    return ExecuteMouseReadOperation(_MouseReadInputParam);
                default:
                    throw new ArgumentOutOfRangeException(nameof(aInputParam));
            }

            MouseActionExecutionResult _MouseActionExecutionResult = new()
            {
                Success = true
            };

            return _MouseActionExecutionResult;
        }

        private static IInputExecutionResult ExecuteMouseReadOperation(IMouseReadInputParam aMouseReadInputParam)
        {
            MouseReadExecutionResult _MouseActionExecutionResult = new();

            Win32Helper.GetCursorPos(out Point _CursorLocation);

            _MouseActionExecutionResult.X = _CursorLocation.X;
            _MouseActionExecutionResult.Y = _CursorLocation.Y;

            _MouseActionExecutionResult.Success = true;
            return _MouseActionExecutionResult;
        }

        // ********************************************************************
        //                            Internal
        // ********************************************************************
        internal static IInputExecutionResult Execute(InputsManager.InputType aInputType, IInputParam aInputParam)
        {
            return aInputType switch
            {
                InputsManager.InputType.Mouse => ExecuteMouseOperation(aInputParam as IMouseInputParam),
                InputsManager.InputType.Keyboard => ExecuteKeyboardOperation(aInputParam as IKeyboardInputParam),
                _ => throw new ArgumentOutOfRangeException(nameof(aInputType), aInputType, null)
            };
        }
    }
}
