using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Radiant.Common.Diagnostics;
using RadiantInputsManager.ExecutionResults;
using RadiantInputsManager.InputsParam;

namespace RadiantInputsManager.Linux.xdotool
{
    public static class XdoToolInputsManager
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static IInputKeyboardExecutionResult ExecuteKeyboardOperation(IKeyboardInputParam aInputParam) // TODO: return will be IInputKeyboardExecutionResult
        {
            if (aInputParam == null)
                return null;

            ProcessStartInfo _StartInfo = new() { FileName = "/bin/bash", Arguments = GenerateXdoToolCommandLineFromArgs(aInputParam) };
            Process _Process = new()
            {
                StartInfo = _StartInfo
            };

            _Process.Start();

            InputKeyboardExecutionResult _InputKeyboardExecutionResult = new()
            {
                Success = true,
            };

            return _InputKeyboardExecutionResult;
        }

        private static IInputMouseExecutionResult ExecuteMouseOperation(IMouseInputParam aInputParam)
        {
            if (aInputParam == null)
                return null;

            ProcessStartInfo _StartInfo = new() { FileName = "/bin/bash", Arguments = GenerateXdoToolCommandLineFromArgs(aInputParam) };
            Process _Process = new();

            // Read input is a little special as we need to read shell output
            if (aInputParam is IMouseReadInputParam)
            {
                _StartInfo.RedirectStandardOutput = true;
                _StartInfo.RedirectStandardError = true;
                _StartInfo.UseShellExecute = false;
                _Process.StartInfo = _StartInfo;

                string _Output = "";
                string _ErrorOutput = "";

                _Process.Start();
                using (StreamReader myOutput = _Process.StandardOutput)
                {
                    _Output = myOutput.ReadToEnd();
                }

                using (StreamReader myError = _Process.StandardError)
                {
                    _ErrorOutput = myError.ReadToEnd();
                }

                if (!string.IsNullOrWhiteSpace(_ErrorOutput))
                {
                    LoggingManager.LogToFile("968A5B47-A869-4230-95C1-2E393738CA5B", $"Couldn't read mouse location. See error: [{_ErrorOutput}].");
                    throw new Exception($"Couldn't read mouse location. See error: [{_ErrorOutput}].");
                }

                // Build execution result
                string[] _Lines = _Output.Split(Environment.NewLine);

                if (_Lines.Length < 2)
                {
                    LoggingManager.LogToFile("23A97477-6D0B-4F51-B5E3-8D260B991702", $"Couldn't read mouse location. Nb Lines: [{_Lines.Length}].");
                    throw new Exception($"Couldn't read mouse location. Nb Lines: [{_Lines.Length}].");
                }

                string _StrX = _Lines[0][2..];
                string _StrY = _Lines[1][2..];

                if (!int.TryParse(_StrX, out int _X) || !int.TryParse(_StrY, out int _Y))
                {
                    LoggingManager.LogToFile("332AF934-1405-4318-8B62-F46E099BD353", $"Couldn't read mouse location. X=[{_StrX}], Y=[{_StrY}]");
                    throw new Exception($"Couldn't read mouse location. X=[{_StrX}], Y=[{_StrY}]");
                }

                MouseReadExecutionResult _MouseReadExecutionResult = new()
                {
                    Success = string.IsNullOrWhiteSpace(_ErrorOutput),
                    X = _X,
                    Y = _Y
                };

                return _MouseReadExecutionResult;
            }

            _Process.StartInfo = _StartInfo;
            _Process.Start();

            MouseActionExecutionResult _MouseActionExecutionResult = new()
            {
                Success = true,
            };

            return _MouseActionExecutionResult;
        }

        private static string GenerateXdoToolCommandLineFromArgs(IMouseInputParam aInputParam)
        {
            string _XdoToolArgs = " -c \"xdotool";

            _XdoToolArgs += aInputParam switch
            {
                IMouseActionInputParam _MouseActionInputParam => GenerateXdoToolCommandLineFromMouseActionArgs(_MouseActionInputParam),
                IMouseReadInputParam _MouseReadInputParam => GenerateXdoToolCommandLineFromMouseReadArgs(_MouseReadInputParam),
                _ => throw new ArgumentOutOfRangeException(nameof(aInputParam))
            };

            _XdoToolArgs += "\"";

            return _XdoToolArgs;
        }

        private static string GenerateXdoToolCommandLineFromArgs(IKeyboardInputParam aInputParam)
        {
            string _XdoToolArgs = " -c \"xdotool";

            _XdoToolArgs += aInputParam switch
            {
                IKeyboardKeyStrokeActionInputParam _KeyboardKeyStrokeActionInputParam => GenerateXdoToolCommandLineFromKeyboardKeystrokeArgs(_KeyboardKeyStrokeActionInputParam),
                IKeyboardTypeActionInputParam _KeyboardTypeActionInputParam => GenerateXdoToolCommandLineFromKeyboardKeystrokeArgs(_KeyboardTypeActionInputParam),
                _ => throw new ArgumentOutOfRangeException(nameof(aInputParam))
            };

            _XdoToolArgs += "\"";

            return _XdoToolArgs;
        }

        private static string GenerateXdoToolCommandLineFromKeyboardKeystrokeArgs(IKeyboardKeyStrokeActionInputParam aInputParam)
        {
            if (aInputParam.KeyStrokeCodes.Length <= 0)
                throw new Exception("Tried to execute null keystroke.");

            string _XdoToolArgs = " key";

            if (aInputParam.Delay != null)
                _XdoToolArgs += $" --delay {aInputParam.Delay.Value}";

            _XdoToolArgs += $" {(byte)aInputParam.KeyStrokeCodes[0]}";

            foreach (Keycode _KeyStroke in aInputParam.KeyStrokeCodes.Skip(1))
                _XdoToolArgs += $"+{(byte)_KeyStroke}";

            return _XdoToolArgs;
        }

        private static string GenerateXdoToolCommandLineFromKeyboardKeystrokeArgs(IKeyboardTypeActionInputParam aInputParam)
        {
            string _XdoToolArgs = " type"; // TODO: escape '

            if (aInputParam.Delay != null)
                _XdoToolArgs += $" --delay {aInputParam.Delay.Value}";

            _XdoToolArgs += $" '{aInputParam.ValueToType}'";

            return _XdoToolArgs;
        }

        private static string GenerateXdoToolCommandLineFromMouseActionArgs(IMouseActionInputParam aInputParam)
        {
            string _XdoToolArgs = "";

            if (aInputParam.Button != MouseOptions.MouseButtons.None)
                _XdoToolArgs += $" click {(int)aInputParam.Button}";
            else
                _XdoToolArgs += $" mousemove {aInputParam.X} {aInputParam.Y}";

            return _XdoToolArgs;
        }

        private static string GenerateXdoToolCommandLineFromMouseReadArgs(IMouseReadInputParam _)
        {
            return " getmouselocation --shell";
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
