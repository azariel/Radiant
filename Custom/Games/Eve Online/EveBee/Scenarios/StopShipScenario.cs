using Radiant.InputsManager.InputsParam;

namespace EveBee.Scenarios
{
    public class StopShipScenario
    {
        public KeyboardKeyStrokeActionInputParam StopShip { get; set; }

        public int NbMsToWaitForStopping { get; set; } = 3000;
    }
}
