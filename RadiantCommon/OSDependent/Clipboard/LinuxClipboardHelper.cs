using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Radiant.Common.OSDependent.Clipboard
{
    internal static class LinuxClipboardHelper
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        private static class BashRunner
        {
            //To work around https://github.com/dotnet/runtime/issues/27128
            private static bool DoubleWaitForExit(Process process)
            {
                var result = process.WaitForExit(500);
                if (result)
                    process.WaitForExit();
                return result;
            }

            public static string Run(string commandLine)
            {
                StringBuilder errorBuilder = new();
                StringBuilder outputBuilder = new();
                var arguments = $"-c \"{commandLine}\"";
                using Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = "bash",
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    }
                };
                process.Start();
                process.OutputDataReceived += (_, args) => { outputBuilder.AppendLine(args.Data); };
                process.BeginOutputReadLine();
                process.ErrorDataReceived += (_, args) => { errorBuilder.AppendLine(args.Data); };
                process.BeginErrorReadLine();
                if (!DoubleWaitForExit(process))
                {
                    var timeoutError = $@"Process timed out. Command line: bash {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
                    throw new(timeoutError);
                }

                if (process.ExitCode == 0)
                    return outputBuilder.ToString();

                var error = $@"Could not execute process. Command line: bash {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
                throw new(error);
            }
        }

        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        static LinuxClipboardHelper()
        {
            isWsl = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") != null;
        }

        private static readonly bool isWsl;

        private static void InnerGetText(string tempFileName)
        {
            if (isWsl)
                BashRunner.Run($"powershell.exe Get-Clipboard  > {tempFileName}");
            else
                BashRunner.Run($"xsel -o --clipboard  > {tempFileName}");
        }

        private static void InnerSetText(string tempFileName)
        {
            try
            {
                if (isWsl)
                    BashRunner.Run($"cat {tempFileName} | clip.exe ");
                else
                    BashRunner.Run($"cat {tempFileName} | xsel -i --clipboard ");
            } finally
            {
                File.Delete(tempFileName);
            }
        }

        public static string? GetText()
        {
            var tempFileName = Path.GetTempFileName();
            try
            {
                InnerGetText(tempFileName);
                return File.ReadAllText(tempFileName);
            } finally
            {
                File.Delete(tempFileName);
            }
        }

        public static async Task<string?> GetTextAsync(CancellationToken cancellation)
        {
            var tempFileName = Path.GetTempFileName();
            try
            {
                InnerGetText(tempFileName);
                return await File.ReadAllTextAsync(tempFileName, cancellation);
            } finally
            {
                File.Delete(tempFileName);
            }
        }

        public static void SetText(string text)
        {
            var tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, text);
            InnerSetText(tempFileName);
        }

        public static async Task SetTextAsync(string text, CancellationToken cancellation)
        {
            var tempFileName = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFileName, text, cancellation);

            if (cancellation.IsCancellationRequested)
                return;

            InnerSetText(tempFileName);
        }
    }
}
