using Radiant.InputsManager.InputsParam;

namespace EveBee.Scenarios
{
    public class RepairShipsScenario
    {
        public int RepairCooldownInMin { get; set; }
        public List<KeyboardKeyStrokeActionInputParam> RepairShipsWhenDocked { get; set; }
        public bool Enabled { get; set; }
    }
}
