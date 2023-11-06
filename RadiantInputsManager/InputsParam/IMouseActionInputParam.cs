using Radiant.InputsManager.Linux.xdotool;

namespace Radiant.InputsManager.InputsParam
{
    internal interface IMouseActionInputParam : IMouseInputParam
    {
        //bool GoBackToCenterScreenFirst { get; set; }
        MouseOptions.MouseButtons Button { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}
