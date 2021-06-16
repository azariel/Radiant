using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using RadiantInputsManager.InputsParam;
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
        private const byte KEYCODE_VK_RETURN = 0x0D;
        private const byte KEYCODE_VK_F11 = 0x7A;
        private const byte KEYCODE_VK_U = 0x55;
        private const byte KEYCODE_VK_A = 0x41;
        private const byte KEYCODE_VK_C = 0x43;
        private const byte KEYCODE_VK_W = 0x57;
        private const byte KEYCODE_VK_CONTROL = 0x11;

        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;

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
            byte _KeyCode = aKeyCode switch
            {
                Keycode.KP_Enter => KEYCODE_VK_RETURN,
                Keycode.XK_F11 => KEYCODE_VK_F11,
                Keycode.CtrlL => KEYCODE_VK_CONTROL,
                Keycode.XK_u => KEYCODE_VK_U,
                Keycode.XK_a => KEYCODE_VK_A,
                Keycode.XK_c => KEYCODE_VK_C,
                Keycode.XK_w => KEYCODE_VK_W,
                _ => throw new ArgumentOutOfRangeException(nameof(aKeyCode), aKeyCode, null)
            };

            uint _FlagByAction = aKeyStrokeAction == KeyStrokeAction.Press ? 0 : KEYEVENTF_KEYUP;
            keybd_event(_KeyCode, 0, _FlagByAction, (UIntPtr)0);
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

        public static void ExecuteKeyboardTextType(string aTextToType, int aDelayBetweenType = 30)
        {
            foreach (char _Char in aTextToType)
            {
                keybd_event(0, Convert.ToByte(_Char), KEYEVENTF_UNICODE, (UIntPtr)0);
                Thread.Sleep(32);
                keybd_event(0, Convert.ToByte(_Char), KEYEVENTF_UNICODE | KEYEVENTF_KEYUP, (UIntPtr)0);
                Thread.Sleep(aDelayBetweenType);
            }
        }
    }
}