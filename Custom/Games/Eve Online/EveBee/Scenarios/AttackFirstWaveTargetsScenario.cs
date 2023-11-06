using Radiant.InputsManager.InputsParam;

namespace EveBee.Scenarios
{
    public class AttackFirstWaveTargetsScenario
    {
        public List<KeyboardKeyStrokeActionInputParam> AttackCurrentTarget { get; set; }
        public List<MouseActionInputParam> SelectTopTarget { get; set; }
        public int NbManualTargetsToDo { get; set; }
        public int TargetDelayMs { get; set; }
        public int DamageTargetDelayMs { get; set; }
    }
}
