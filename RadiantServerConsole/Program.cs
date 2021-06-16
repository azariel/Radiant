using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Radiant.Common.Configuration;
using Radiant.Common.Tasks;
using Radiant.Common.Tasks.Triggers;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;
using RadiantInputsManager.Linux.xdotool;
using RadiantInputsManager.Tasks;

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
            Console.WriteLine("1) Inputs Manager");

            ConsoleKeyInfo _Key = Console.ReadKey();

            switch (_Key.Key)
            {
                case ConsoleKey.D0 or ConsoleKey.NumPad0:
                    Environment.Exit(0);
                    break;
                case ConsoleKey.D1 or ConsoleKey.NumPad1:
                    Console.Clear();
                    Console.WriteLine("-----Inputs Manager-----");
                    Console.WriteLine("---Status---");

                    RadiantConfig _test = new();
                    _test.Tasks.Tasks.Add(new ManualAutomationTask
                    {
                        Triggers = new List<ITrigger>
                        {
                            new ScheduleTrigger
                            {
                                TriggerEverySeconds = 10,
                                //ResetTriggeredTimesOnStart = true
                            }
                        },
                        Settings = new List<ManualAutomationSetting>
                        {
                            new()
                            {
                                ManualAutomationOperationType = ManualAutomation.ManualAutomationOperationType.MouseClick,
                                InputParam = new MouseActionInputParam
                                {
                                    X = 5,
                                    Y = 6,
                                    Button = MouseOptions.MouseButtons.Left
                                }
                            }
                        },
                        IsEnabled = true
                    });
                    CommonConfigurationManager.SetConfigInMemory(_test);
                    CommonConfigurationManager.SaveConfigInMemoryToDisk();

                    RadiantConfig _RadiantConfig = CommonConfigurationManager.ReloadConfig();

                    break;
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
