using Radiant.InputsManager.InputsParam;

namespace EveBee.Scenarios
{
    public class FindNextCombatSiteScenario
    {
        public string AnomalyNameEnforcerLower { get; set; }
        public List<KeyboardKeyStrokeActionInputParam> GetRadarData { get; set; }
        public List<MouseActionInputParam> CleanTopOneRadarResult { get; set; }
    }
}
