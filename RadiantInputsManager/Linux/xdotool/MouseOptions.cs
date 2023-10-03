namespace Radiant.InputsManager.Linux.xdotool
{
    public class MouseOptions
    {
        // ********************************************************************
        //                            Nested Types
        // ********************************************************************
        /// <summary>
        /// Highly tied to int values from xdoTool http://manpages.ubuntu.com/manpages/trusty/man1/xdotool.1.html#mouse%20commands
        /// </summary>
        public enum MouseButtons
        {
            None,
            Left,
            Middle,
            Right,
            WheelUp,
            WheelDown
        }
    }
}
