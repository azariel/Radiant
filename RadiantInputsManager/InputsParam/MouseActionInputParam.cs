using Radiant.InputsManager.Linux.xdotool;

namespace Radiant.InputsManager.InputsParam
{
    public class MouseActionInputParam : IMouseActionInputParam
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public MouseOptions.MouseButtons Button { get; set; }
        //public bool GoBackToCenterScreenFirst { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
