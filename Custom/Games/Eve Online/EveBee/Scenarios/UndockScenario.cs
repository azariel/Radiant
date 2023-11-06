using Radiant.InputsManager;
using Radiant.InputsManager.InputsParam;

namespace EveBee.Scenarios
{
    public class UndockScenario
    {
        public KeyboardKeyStrokeActionInputParam Undock { get; set; }

        public int NbMsToWaitForUndock { get; set; } = 10000;
    }
}
