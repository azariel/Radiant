using Radiant.InputsManager.InputsParam;

namespace EveBee.Scenarios
{
    public class AttackFirstWaveTargetsScenario
    {
        public List<KeyboardKeyStrokeActionInputParam> AttackCurrentTarget { get; set; }
        public List<MouseActionInputParam> SelectTopTarget { get; set; }
        public int DelayBetweenDroneAttackNewTarget { get; set; }
        //public int PixelsSpacingBetweenTargetsInOverview { get; set; }
        public List<MouseActionInputParam> TargetSelectedEnemy { get; set; }
        public int ContinuousAttackDurationInLoop { get; set; }
    }
}
