using System;
using System.Runtime.InteropServices;

namespace Radiant.Common.OSDependent
{
    public static class OperatingSystemHelper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static SupportedOperatingSystem GetCurrentOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SupportedOperatingSystem.Linux;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return SupportedOperatingSystem.Windows;

            throw new ArgumentOutOfRangeException("Current operating system isn't handled. Please report this to the administrator.");
        }
    }
}
