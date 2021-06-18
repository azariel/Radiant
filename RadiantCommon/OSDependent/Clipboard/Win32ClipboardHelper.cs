using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Radiant.Common.OSDependent.Clipboard
{
    internal static class Win32ClipboardHelper
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private const uint CF_UNICODE_TEXT = 13;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern int GlobalSize(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        private static string? InnerGet()
        {
            IntPtr handle = default;

            IntPtr pointer = default;
            try
            {
                handle = GetClipboardData(CF_UNICODE_TEXT);
                if (handle == default)
                    return null;

                pointer = GlobalLock(handle);
                if (pointer == default)
                    return null;

                var size = GlobalSize(handle);
                var buff = new byte[size];

                Marshal.Copy(pointer, buff, 0, size);

                return Encoding.Unicode.GetString(buff).TrimEnd('\0');
            } finally
            {
                if (pointer != default)
                    GlobalUnlock(handle);

                CloseClipboard();
            }
        }

        private static void InnerSet(string text)
        {
            EmptyClipboard();
            IntPtr hGlobal = default;
            try
            {
                var bytes = (text.Length + 1) * 2;
                hGlobal = Marshal.AllocHGlobal(bytes);

                if (hGlobal == default)
                    ThrowWin32();

                var target = GlobalLock(hGlobal);

                if (target == default)
                    ThrowWin32();

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                } finally
                {
                    GlobalUnlock(target);
                }

                if (SetClipboardData(CF_UNICODE_TEXT, hGlobal) == default)
                    ThrowWin32();

                hGlobal = default;
            } finally
            {
                if (hGlobal != default)
                    Marshal.FreeHGlobal(hGlobal);

                CloseClipboard();
            }
        }

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        private static void ThrowWin32()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        private static void TryOpenClipboard()
        {
            var num = 10;
            while (true)
            {
                if (OpenClipboard(default))
                    break;

                if (--num == 0)
                    ThrowWin32();

                Thread.Sleep(100);
            }
        }

        private static async Task TryOpenClipboardAsync(CancellationToken cancellation)
        {
            var num = 10;
            while (true)
            {
                if (OpenClipboard(default))
                    break;

                if (--num == 0)
                    ThrowWin32();

                await Task.Delay(100, cancellation);
            }
        }

        public static string? GetText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODE_TEXT))
                return null;

            TryOpenClipboard();

            return InnerGet();
        }

        public static async Task<string?> GetTextAsync(CancellationToken cancellation)
        {
            if (!IsClipboardFormatAvailable(CF_UNICODE_TEXT))
                return null;

            await TryOpenClipboardAsync(cancellation);

            return InnerGet();
        }

        public static void SetText(string text)
        {
            TryOpenClipboard();

            InnerSet(text);
        }

        public static async Task SetTextAsync(string text, CancellationToken cancellation)
        {
            await TryOpenClipboardAsync(cancellation);

            InnerSet(text);
        }
    }
}
