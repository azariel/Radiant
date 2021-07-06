namespace RadiantInputsManager.ExecutionResults
{
    public class MouseReadExecutionResult : IInputMouseReadExecutionResult
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool Success { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
