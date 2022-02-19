using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EveRay
{
    public static class WindowHelper
    {
        //[STAThread]
        //public static Screen GetCurrentScreen(Window window)
        //{
        //    var parentArea = new System.Drawing.Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
        //    return Screen.FromRectangle(parentArea);
        //}

        //[STAThread]
        //public static Screen GetScreen(int requestedScreen)
        //{
        //    var screens = Screen.AllScreens;
        //    var mainScreen = 0;
        //    if (screens.Length > 1 && mainScreen < screens.Length)
        //    {
        //        return screens[requestedScreen];
        //    }
        //    return screens[0];
        //}
    }
}
