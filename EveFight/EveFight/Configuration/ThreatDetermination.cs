namespace EveFight.Configuration;

public class ThreatDetermination
{
    public int PrioritizeTackleIfNumberOfTacklingShipsBelowThisNumber { get; set; } = 1;
    public int PrioritizeLogiShipsIfDpsBelowThisNumber { get; set; } = 1000;
}
