namespace Radiant.InputsManager.InputsParam
{
    internal interface IKeyboardKeyStrokeActionInputParam : IKeyboardInputParam
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public int? Delay { get; set; }
        public Keycode[] KeyStrokeCodes { get; set; }
    }
}
