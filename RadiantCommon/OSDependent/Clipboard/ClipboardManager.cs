using System;
using System.Threading;

namespace Radiant.Common.OSDependent.Clipboard
{
    public static class ClipboardManager
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string GetClipboardValue()
        {
            SupportedOperatingSystem _OperatingSystem = OperatingSystemHelper.GetCurrentOperatingSystem();
            switch (_OperatingSystem)
            {
                case SupportedOperatingSystem.Linux:
                    return LinuxClipboardHelper.GetText();
                case SupportedOperatingSystem.Windows:
                    return Win32ClipboardHelper.GetText();
                default:
                    throw new ArgumentOutOfRangeException($"Getting clipboard value from Operating system [{_OperatingSystem}] isn't supported.");
            }
        }

        public static void SetClipboardValue(string aValue, bool aWaitOnEnd = true)
        {
            SupportedOperatingSystem _OperatingSystem = OperatingSystemHelper.GetCurrentOperatingSystem();
            switch (_OperatingSystem)
            {
                case SupportedOperatingSystem.Linux:
                    LinuxClipboardHelper.SetText(aValue);
                    break;
                case SupportedOperatingSystem.Windows:
                    Win32ClipboardHelper.SetText(aValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Getting clipboard value from Operating system [{_OperatingSystem}] isn't supported.");
            }

            if(aWaitOnEnd)
                Thread.Sleep(50);
        }
    }
}
