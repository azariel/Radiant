using Radiant.InputsManager.InputsParam;

namespace EveBee.Scenarios
{
    public class DockToStationScenario
    {
        public List<MouseActionInputParam> AlignToStation { get; set; }
        // TODO: deactivate afterburner/modules
        public List<KeyboardKeyStrokeActionInputParam> RecallDrones { get; set; }
        public List<KeyboardKeyStrokeActionInputParam> DeactivateModules { get; set; }
        public List<MouseActionInputParam> DockToStation { get; set; }
        public int WaitForDroneDelayMs { get; set; }
    }
}
