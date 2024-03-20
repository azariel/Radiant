using System;
using Radiant.Common.Tasks.Triggers;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Radiant.Common.Tests.Triggers
{
    public class ScheduleTriggerTest
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void BasicScheduleTest()
        {
            ScheduleTrigger _Trigger = new()
            {
                TriggerEveryXSeconds = 5
            };

            bool _FirstEval = _Trigger.Evaluate();

            // We just set the TriggerEveryXSeconds of a newly created trigger. It hasn't been 5 seconds yet, so it should'nt trigger
            Assert.False(_FirstEval);

            Stopwatch _StopWatch = new();
            _StopWatch.Start();
            for (int i = 0; i < 100; i++)// for 10 sec
            {
                if (_StopWatch.ElapsedMilliseconds > _Trigger.TriggerEveryXSeconds * 1000)
                {
                    bool _EvalTrue = _Trigger.Evaluate();
                    Assert.True(_EvalTrue);

                    // Don't re-trigger immediately after
                    bool _EvalFalse = _Trigger.Evaluate();
                    Assert.False(_EvalFalse);
                    break;
                }
                else if (_StopWatch.ElapsedMilliseconds < (_Trigger.TriggerEveryXSeconds * 1000) * 0.8)// *.8 to give time for evaluation,a void failing test from time to time
                {
                    bool _EvalFalse = _Trigger.Evaluate();
                    Assert.False(_EvalFalse);
                }

                Thread.Sleep(100);
            }
        }

        [Fact]
        public void BasicScheduleWithActiveBlackoutTest()
        {
            DateTime _NowDateTime = DateTime.UtcNow;
            DateTime _MaxDateTime = _NowDateTime.AddMinutes(5);
            ScheduleTrigger _Trigger = new()
            {
                TriggerEveryXSeconds = 5,
                BlackOutTimeFrame = new ScheduleTrigger.ScheduleTriggerBlackOutTimeFrame()
                {
                    BlackOutStart = new TimeSpan(_NowDateTime.Hour, _NowDateTime.Minute, _NowDateTime.Second),
                    BlackOutEnd = new TimeSpan(_MaxDateTime.Hour, _MaxDateTime.Minute, _MaxDateTime.Second)
                }
            };

            bool _FirstEval = _Trigger.Evaluate();

            // We just set the TriggerEveryXSeconds of a newly created trigger. It hasn't been 5 seconds yet, so it should'nt trigger
            Assert.False(_FirstEval);

            Stopwatch _StopWatch = new();
            _StopWatch.Start();
            for (int i = 0; i < 100; i++)// for 10 sec
            {
                Assert.False(_Trigger.Evaluate());
                Thread.Sleep(100);
            }
        }
    }
}
