namespace Radiant.Custom.Games.EveOnline.EveFight.Configuration;

public class ThreatDetermination
{
    public int PrioritizeTackleIfNumberOfTacklingShipsBelowThisNumberAndTankIsGreen { get; set; } = 1;
    public int PrioritizeLogiShipsIfDpsBelowThisNumber { get; set; } = 1000;
}
