namespace Radiant.InputsManager.ExecutionResults
{
    public interface IInputMouseReadExecutionResult : IInputMouseExecutionResult 
    {
        int X { get; set; }
        int Y { get; set; }
    }
}
