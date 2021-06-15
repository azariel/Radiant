using System;
using System.Collections.Generic;
using System.Drawing;
using Radiant.Common.Tasks.Triggers;
using RadiantInputsManager.InputsParam;

namespace RadiantInputsManager.Tasks
{
    public class ManualAutomationTask : RadiantTask
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation() 
        {
            foreach (ManualAutomationSetting _Setting in this.Settings)
            {
                switch (_Setting.ManualAutomationOperationType)
                {
                    case ManualAutomation.ManualAutomationOperationType.KeyboardExecute:
                        var _ParamKeyStroke = (IKeyboardKeyStrokeActionInputParam)_Setting.InputParam;
                        ManualAutomation.KeyboardExecute(_ParamKeyStroke.KeyStrokeCodes, _ParamKeyStroke.Delay);
                        break;
                    case ManualAutomation.ManualAutomationOperationType.KeyboardType:
                        var _ParamKeyInput = (IKeyboardTypeActionInputParam)_Setting.InputParam;
                        ManualAutomation.KeyboardType(_ParamKeyInput.ValueToType, _ParamKeyInput.Delay ?? 20, _ParamKeyInput.Delay ?? 100);
                        break;
                    case ManualAutomation.ManualAutomationOperationType.MouseClick:
                        var _ParamMouseClick = (IMouseActionInputParam)_Setting.InputParam;
                        ManualAutomation.MouseClick(new Point(_ParamMouseClick.X, _ParamMouseClick.Y), _ParamMouseClick.Button);
                        break;
                    case ManualAutomation.ManualAutomationOperationType.MoveCursorToLocation:
                        var _ParamMouseMove = (IMouseActionInputParam)_Setting.InputParam;
                        ManualAutomation.MoveCursorToLocation(new Point(_ParamMouseMove.X, _ParamMouseMove.Y));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"E6A15324-0DC4-4557-BBBC-F93954059D94");
                }
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<ManualAutomationSetting> Settings { get; set; }
    }
}
