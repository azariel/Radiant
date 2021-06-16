namespace Radiant.Common.Tasks.Triggers
{
    public interface ITrigger
    {
        bool Evaluate();
        string UID { get; set; }
    }
}
