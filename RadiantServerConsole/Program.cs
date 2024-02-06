using Radiant.Common.Configuration;
using Radiant.Common.Tasks;
using Radiant.Common.Utils;
using Radiant.Servers.RadiantServerConsole.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Radiant.Servers.RadiantServerConsole
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
        private static readonly object Lock = new();

        private static void Main(string[] args)
        {
            StartBackgroundProcesses();
            Console.WriteLine("-----Please choose and option-----");
            Console.WriteLine("0) Exit");

            ConsoleKeyInfo _Key = Console.ReadKey();
            switch (_Key.Key)
            {
                case ConsoleKey.D0 or ConsoleKey.NumPad0:
                    Environment.Exit(0);
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
            // Register tasks in custom libraries
            RadiantServerConsoleConfiguration _ServerConfig = RadiantConsoleConfigurationManager.ReloadConfig();
            AssemblyUtils.LoadAssemblyFilesToMemory(_ServerConfig.DependentLibraries);

            RadiantConfig _RadiantConfig = CommonConfigurationManager.ReloadConfig();

            if (_RadiantConfig == null)
                return;

            while (true)
            {
                // Evaluate each tasks async to avoid blocking evaluation of other tasks
                foreach (IRadiantTask _RadiantTask in _RadiantConfig.Tasks.Tasks.Where(w => w.IsEnabled && w.State == TaskState.Idle))
                {
                    // If there's a running foreground task, ignore all tasks requiring foreground exclusivity

                    lock (Lock)
                    {
                        lock (_RadiantTask.TaskLockObject)
                        {
                            if (_RadiantTask.State == TaskState.InProgress)
                                continue;

                            _RadiantTask.State = TaskState.InTriggerEvaluation;
                        }
                    }

                    // Copy reference for our new Task
                    IRadiantTask _CurrentRadiantTask = _RadiantTask;
                    Task.Run(() =>
                    {
                        _CurrentRadiantTask.EvaluateTriggers(() =>
                        {
                            lock (Lock)
                            {
                                IRadiantTask _TaskFromLiveList = _RadiantConfig.Tasks.Tasks.Single(s => s.UID == _CurrentRadiantTask.UID);
                                lock (_TaskFromLiveList.TaskLockObject)
                                {
                                    if (_TaskFromLiveList.State != TaskState.InTriggerEvaluation)
                                        return false;

                                    if (_RadiantConfig.Tasks.Tasks.Any(a =>
                                            a.IsEnabled &&
                                            a.State == TaskState.InProgress &&
                                            a.IsForegroundExclusive))
                                    {
                                        return false;
                                    }

                                    _TaskFromLiveList.State = TaskState.InProgress;
                                    return true;
                                }
                            }
                        }, null);
                    });
                }

                Thread.Sleep(CORE_LOOP_MS);
            }
        }
    }
}
