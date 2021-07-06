using RadiantInputsManager.Linux.xdotool;

namespace RadiantInputsManager.InputsParam
{
    public class MouseActionInputParam : IMouseActionInputParam
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public MouseOptions.MouseButtons Button { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
