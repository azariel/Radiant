namespace EveFight.Models
{
    public class WeaponDefinition
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string WeaponType { get; set; }
        //public AmmunitionDefinition {get;set;}//TODO
        public WeaponRangeStyle DefaultRange { get; set; }
    }
}
