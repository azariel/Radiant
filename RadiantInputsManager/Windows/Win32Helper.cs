using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using RadiantInputsManager.Linux.xdotool;

namespace RadiantInputsManager.Windows
{
    public static class Win32Helper
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum KeyStrokeAction
        {
            Press,
            Release
        }

        // ********************************************************************
        //                            Constants
        // ********************************************************************
        //https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN

        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_HWHEEL = 0x01000;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
        private const uint MOUSEEVENTF_XDOWN = 0x0080;
        private const uint MOUSEEVENTF_XUP = 0x0100;

        private static void ExecuteMouseEventByButton(uint aButtonAliasDown, uint aButtonAliasUp, int aDelay = 100)
        {
            ExecuteMouseEventByButton(aButtonAliasDown);
            Thread.Sleep(aDelay);
            ExecuteMouseEventByButton(aButtonAliasUp);
        }

        private static void ExecuteMouseEventByButton(uint aButtonAlias)
        {
            mouse_event(aButtonAlias, 0, 0, 0, (UIntPtr)0);
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // ********************************************************************
        //                            Private
        // ********************************************************************
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void ExecuteKeyboardKey(Keycode aKeyCode, KeyStrokeAction aKeyStrokeAction)
        {
            Keycode[] _ExtendedKeycodes =
            {
                
                Keycode.XK_Shift_L,
                Keycode.XK_Right,
                Keycode.XK_Left,
                Keycode.XK_Down,
                Keycode.XK_Up,
                Keycode.XK_End,
                Keycode.XK_Home,
            };

            byte _KeyCode = aKeyCode switch
            {
                Keycode.KP_Enter => (byte)WindowsVirtualKeys.Return,
                Keycode.XK_F11 => (byte)WindowsVirtualKeys.F11,
                Keycode.XK_F12 => (byte)WindowsVirtualKeys.F12,
                Keycode.XK_TAB => (byte)WindowsVirtualKeys.Tab,
                Keycode.XK_Control_L => (byte)WindowsVirtualKeys.LeftControl,
                Keycode.XK_Shift_L => (byte)WindowsVirtualKeys.LeftShift,
                Keycode.XK_Left => (byte)WindowsVirtualKeys.Left,
                Keycode.XK_Right => (byte)WindowsVirtualKeys.Right,
                Keycode.XK_Down => (byte)WindowsVirtualKeys.Down,
                Keycode.XK_Up => (byte)WindowsVirtualKeys.Up,
                Keycode.XK_Escape => (byte)WindowsVirtualKeys.Escape,
                Keycode.XK_u => (byte)WindowsVirtualKeys.U,
                Keycode.XK_a => (byte)WindowsVirtualKeys.A,
                Keycode.XK_c => (byte)WindowsVirtualKeys.C,
                Keycode.XK_f => (byte)WindowsVirtualKeys.F,
                Keycode.XK_w => (byte)WindowsVirtualKeys.W,
                Keycode.XK_End => (byte)WindowsVirtualKeys.End,
                Keycode.XK_Home => (byte)WindowsVirtualKeys.Home,
                _ => throw new ArgumentOutOfRangeException(nameof(aKeyCode), aKeyCode, null)
            };

            // Note that KEYEVENTF_EXTENDEDKEY flag is required for Numpad, LSHIFT, etc
            uint _FlagByAction;

            if (_ExtendedKeycodes.Any(a => a == aKeyCode))
                _FlagByAction = aKeyStrokeAction == KeyStrokeAction.Press ? KEYEVENTF_EXTENDEDKEY : KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            else
                _FlagByAction = aKeyStrokeAction == KeyStrokeAction.Press ? 0 : KEYEVENTF_KEYUP;

            File.AppendAllText(@"C:\temp\test.txt", $"{DateTime.Now:HH:mm:ss.fff} - Key [{aKeyCode}] [{aKeyStrokeAction}].{Environment.NewLine}");

            keybd_event(_KeyCode, 0, _FlagByAction, (UIntPtr)0);
        }

        public static void ExecuteKeyboardTextType(string aTextToType, int aDelayBetweenType = 30)
        {
            foreach (char _Char in aTextToType)
            {
                keybd_event(0, Convert.ToByte(_Char), KEYEVENTF_UNICODE, (UIntPtr)0);
                Thread.Sleep(64);
                keybd_event(0, Convert.ToByte(_Char), KEYEVENTF_UNICODE | KEYEVENTF_KEYUP, (UIntPtr)0);
                Thread.Sleep(aDelayBetweenType);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static void ExecuteMouseEvent(MouseOptions.MouseButtons aButton)
        {
            switch (aButton)
            {
                case MouseOptions.MouseButtons.None:
                    return;
                case MouseOptions.MouseButtons.Left:
                    ExecuteMouseEventByButton(MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP);
                    break;
                case MouseOptions.MouseButtons.Middle:
                    ExecuteMouseEventByButton(MOUSEEVENTF_MIDDLEDOWN, MOUSEEVENTF_MIDDLEUP);
                    break;
                case MouseOptions.MouseButtons.Right:
                    ExecuteMouseEventByButton(MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_RIGHTUP);
                    break;
                case MouseOptions.MouseButtons.WheelUp:
                    ExecuteMouseEventByButton(MOUSEEVENTF_MIDDLEUP);
                    break;
                case MouseOptions.MouseButtons.WheelDown:
                    ExecuteMouseEventByButton(MOUSEEVENTF_MIDDLEDOWN);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aButton), aButton, null);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static Point GetCursorPosition()
        {
            GetCursorPos(out Point lpPoint);

            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
    }
}
