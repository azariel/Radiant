using RadiantInputsManager.Linux.xdotool;

namespace RadiantInputsManager.InputsParam
{
    internal interface IMouseActionInputParam : IMouseInputParam
    {
        MouseOptions.MouseButtons Button { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}
