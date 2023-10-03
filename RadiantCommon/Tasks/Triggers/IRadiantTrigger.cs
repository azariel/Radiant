namespace Radiant.Common.Tasks.Triggers
{
    public interface IRadiantTrigger
    {
        bool Evaluate();
        string UID { get; set; }
    }
}
