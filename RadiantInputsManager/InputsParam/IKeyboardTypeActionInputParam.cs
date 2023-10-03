namespace Radiant.InputsManager.InputsParam
{
    public interface IKeyboardTypeActionInputParam : IKeyboardInputParam
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public int? Delay { get; set; }
        public string ValueToType { get; set; }
    }
}
