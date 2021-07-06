namespace RadiantInputsManager.InputsParam
{
    public class KeyboardKeyStrokeActionInputParam : IKeyboardKeyStrokeActionInputParam
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public int? Delay { get; set; }
        public Keycode[] KeyStrokeCodes { get; set; }
    }
}
