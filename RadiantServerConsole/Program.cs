using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Radiant.Common.Configuration;
using Radiant.Common.Helpers;
using Radiant.Common.Tasks;
using Radiant.Common.Tasks.Triggers;
using Radiant.ServerConsole.Configuration;

namespace Radiant.ServerConsole
{
    internal class Program
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const int CORE_LOOP_MS = 2000;

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static void Main(string[] args)
        {
            StartBackgroundProcesses();
            Console.WriteLine("-----Please choose and option-----");
            Console.WriteLine("0) Exit");

            //Console.WriteLine("1) Inputs Manager");

            ConsoleKeyInfo _Key = Console.ReadKey();

            switch (_Key.Key)
            {
                case ConsoleKey.D0 or ConsoleKey.NumPad0:
                    Environment.Exit(0);
                    break;

                    //case ConsoleKey.D1 or ConsoleKey.NumPad1:
                    //    Console.Clear();
                    //    Console.WriteLine("-----Inputs Manager-----");
                    //    Console.WriteLine("---Status---");

                    //    RadiantConfig _test = new();
                    //    _test.Tasks.Tasks.Add(new ManualAutomationTask
                    //    {
                    //        Triggers = new List<ITrigger>
                    //        {
                    //            new ScheduleTrigger
                    //            {
                    //                TriggerEverySeconds = 10,
                    //                //ResetTriggeredTimesOnStart = true
                    //            }
                    //        },
                    //        Settings = new List<ManualAutomationSetting>
                    //        {
                    //            new()
                    //            {
                    //                ManualAutomationOperationType = ManualAutomation.ManualAutomationOperationType.MouseClick,
                    //                InputParam = new MouseActionInputParam
                    //                {
                    //                    X = 5,
                    //                    Y = 6,
                    //                    Button = MouseOptions.MouseButtons.Left
                    //                }
                    //            }
                    //        },
                    //        IsEnabled = true
                    //    });

                    //    _test.Tasks.Tasks.Add(new ProductsMonitorTask
                    //    {
                    //        Triggers = new List<ITrigger>
                    //        {
                    //            new ScheduleTrigger
                    //            {
                    //                TriggerEverySeconds = 10,
                    //            }
                    //        },
                    //        IsEnabled = true
                    //    });
                    //    CommonConfigurationManager.SetConfigInMemory(_test);
                    //    CommonConfigurationManager.SaveConfigInMemoryToDisk();

                    //    RadiantConfig _RadiantConfig = CommonConfigurationManager.ReloadConfig();

                    //    break;
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Start all background processes and threads relative to Radiant
        /// </summary>
        private static void StartBackgroundProcesses()
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
        private static void TaskRunnerExecution()
        {
            // Register tasks in custom libraries
            RadiantServerConsoleConfiguration _ServerConfig = RadiantConsoleConfigurationManager.ReloadConfig();
            AssemblyHelper.LoadAssemblyFilesToMemory(_ServerConfig.DependentLibraries);

            RadiantConfig _RadiantConfig = CommonConfigurationManager.ReloadConfig();

            if (_RadiantConfig == null)
                return;

            while (true)
            {
                // Evaluate each tasks async to avoid blocking evaluation of other tasks
                foreach (IRadiantTask _RadiantTask in _RadiantConfig.Tasks.Tasks.Where(w => w.IsEnabled))
                    Task.Run(() => _RadiantTask.EvaluateTriggers());

                Thread.Sleep(CORE_LOOP_MS);
            }
        }
    }
}
