using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Hardcodet.Wpf.TaskbarNotification;
using Radiant.Common.Configuration;
using Radiant.Common.Tasks;
using Radiant.Common.Utils;
using Radiant.WindowsServer.Configuration;
using static System.Net.Mime.MediaTypeNames;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Radiant.WindowsServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const int CORE_LOOP_MS = 2000;

        public MainWindow()
        {
            InitializeComponent();
            StartBackgroundProcesses();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        /// <summary>
        /// Start all background processes and threads relative to Radiant
        /// </summary>
        private void StartBackgroundProcesses()
        {
            Thread _TaskRunnerThread = new(TaskRunnerExecution)
            {
                IsBackground = true
            };
            _TaskRunnerThread.Start();
        }

        /// <summary>
        /// TaskRunnerThread core loop function
        /// </summary>
        private void TaskRunnerExecution()
        {
            // Register tasks in custom libraries
            RadiantWindowsServerConfiguration _ServerConfig = RadiantWindowsServerConfigurationManager.ReloadConfig();
            AssemblyUtils.LoadAssemblyFilesToMemory(_ServerConfig.DependentLibraries);

            RadiantConfig _RadiantConfig = CommonConfigurationManager.ReloadConfig();

            if (_RadiantConfig == null)
                return;

            while (true)
            {
                // Evaluate each tasks async to avoid blocking evaluation of other tasks
                foreach (IRadiantTask _RadiantTask in _RadiantConfig.Tasks.Tasks.Where(w => w.IsEnabled && w.State == TaskState.Idle))
                {
                    lock (_RadiantTask.TaskLockObject)
                    {
                        _RadiantTask.State = TaskState.InProgress;
                    }

                    Task.Run(() =>
                    {
                        _RadiantTask.EvaluateTriggers(() =>
                        {
                            if (_RadiantTask.State == TaskState.InProgress)
                            {
                                ShowBalloon("Task Started", $"Task [{_RadiantTask.GetType().Name}] started.",
                                    BalloonIcon.Info);
                            }
                        });
                    });
                }

                Thread.Sleep(CORE_LOOP_MS);
            }
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OnOpenLogs(object sender, RoutedEventArgs e)
        {
            try
            {
                string _Filename = "radiant.log";
                if (!System.IO.File.Exists(_Filename))
                {
                    MessageBox.Show("logs file doesn't exists.", "File not found", MessageBoxButtons.OK);
                    return;
                }

                Process _OpenFileProcess = new();
                _OpenFileProcess.StartInfo = new()
                {
                    UseShellExecute = true,
                    FileName = _Filename
                };
                _OpenFileProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Couldn't open logs file. Message = [{ex.Message}].");
            }

        }

        private void ShowBalloon(string title, string text, BalloonIcon ballonIcon)
        {
            this.Dispatcher.Invoke(() =>
            {
                //show balloon with built-in icon
                SystemTrayNotificationIcon.ShowBalloonTip(title, text, ballonIcon);
            });
        }
    }
}
