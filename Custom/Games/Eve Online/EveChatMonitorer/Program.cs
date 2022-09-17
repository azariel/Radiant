// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using EveChatMonitorer.Configuration;

Dictionary<string, int> SkipNbFilesByFilePath = new();

void EvaluateFile(FileInfo aFileInfo, EveChatMonitorerConfiguration aConfig)
{
    int _NbLinesToSkip = 0;
    if (SkipNbFilesByFilePath.ContainsKey(aFileInfo.FullName))
        _NbLinesToSkip = SkipNbFilesByFilePath[aFileInfo.FullName];

    string[] _FileLines;
    Stopwatch _Stopwatch = new Stopwatch();
    _Stopwatch.Start();
    int _NbLinesTotal;
    while (true)
    {
        try
        {
            using var fs = new FileStream(aFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            var _Text = sr.ReadToEnd();
            var _AllFileLines = _Text.Split(Environment.NewLine);//.Skip(_NbLinesToSkip).ToArray();
            _NbLinesTotal = _AllFileLines.Length - 1;
            _FileLines = _AllFileLines.Skip(_NbLinesToSkip).ToArray();

            break;
        } catch (Exception _Ex)
        {
            if (_Stopwatch.ElapsedMilliseconds > 10000)
                throw new Exception("Eve FileLog was inaccessible for 10 seconds. Aborting. " + _Ex.Message);

            Thread.Sleep(millisecondsTimeout: 50);
        }
    }

    foreach (var _KeyWorkdNotification in aConfig.KeywordTriggerNotificationCollection)
    {
        if (_FileLines.Any(a => Regex.Match(a, _KeyWorkdNotification.Keyword).Success))
            ExecuteTrigger(_KeyWorkdNotification.NotificationWavFileToPlayOnTrigger);
    }

    _NbLinesToSkip = _NbLinesTotal;
    if (SkipNbFilesByFilePath.ContainsKey(aFileInfo.FullName))
        SkipNbFilesByFilePath[aFileInfo.FullName] = _NbLinesToSkip;
    else
    {
        SkipNbFilesByFilePath.Add(aFileInfo.FullName, _NbLinesToSkip);
    }
}

void ExecuteTrigger(string aNotificationWavFileToPlayOnTrigger)
{
    if (!File.Exists(aNotificationWavFileToPlayOnTrigger))
        return;

    var _SoundPlayer = new SoundPlayer(aNotificationWavFileToPlayOnTrigger);
    _SoundPlayer.Play();
}


var _Config = EveChatMonitorerConfigurationManager.GetConfigFromMemory();

// Validate config
if (string.IsNullOrWhiteSpace(_Config.LogsDirectoryPath))
{
    Console.WriteLine("LogsDirectoryPath is incorrect.");
    Console.ReadKey();
    Environment.Exit(0);
}

if (!Directory.Exists(_Config.LogsDirectoryPath))
    Directory.CreateDirectory(_Config.LogsDirectoryPath);

DirectoryInfo _DirInfo = new DirectoryInfo(_Config.LogsDirectoryPath);
DateTime _StartNow = DateTime.Now;
while (true)
{
    // Check chat logs
    var _AllFiles = _DirInfo.GetFiles();
    Regex _FileNameRegex = new Regex(_Config.KeyLogFileName);
    var _FilesToRefresh = _AllFiles.Where(w => _FileNameRegex.Match(w.Name).Success && w.LastWriteTime > _StartNow.Date);
    foreach (FileInfo _FileInfo in _FilesToRefresh)
        EvaluateFile(_FileInfo, _Config);

    // Wait a bit before refreshing logs
    Thread.Sleep(_Config.RefreshChatsEveryXMs);
}

