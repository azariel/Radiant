//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Radiant.WebScraper.Scrapers.HeadlessChrome
//{
//    public static class HeadlessChromeScraper : IScraper
//    {
//        // ********************************************************************
//        //                            Public
//        // ********************************************************************
//        public static string GetDOMFromUrl(string aUrl)
//        {
//            if (string.IsNullOrWhiteSpace(aUrl))
//                return null;

//            string _Command = @$".\chrome.exe --headless --repl --profile-directory=Default --enable-logging --dump-dom {aUrl}";
//            Process _Process = new()
//            {
//                StartInfo =
//                {
//                    FileName = "powershell.exe",
//                    Arguments = _Command,
//                    CreateNoWindow = true,
//                    UseShellExecute = false,
//                    RedirectStandardOutput = true,
//                    RedirectStandardError = true,
//                    WorkingDirectory = @"C:\Program Files (x86)\Google\Chrome\Application"
//                }
//            };

//            string _Output = "";
//            _Process.OutputDataReceived += (sender, args) =>
//            {
//                _Output += args.Data;
//            };

//            _Process.ErrorDataReceived += (sender, args) =>
//            {
//                _Output += args.Data;
//            };

//            _Process.Start();
//            _Process.BeginOutputReadLine();
//            _Process.BeginErrorReadLine();
//            _Process.WaitForExit();

//            return _Output;
//        }
//    }
//}
