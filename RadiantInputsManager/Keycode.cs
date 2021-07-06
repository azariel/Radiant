using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RadiantInputsManager
{
    // Tightly tied to keysyms https://wiki.linuxquestions.org/wiki/List_of_keysyms or https://www.oreilly.com/library/view/xlib-reference-manual/9780937175262/16_appendix-h.html or https://gitlab.com/cunidev/gestures/-/wikis/xdotool-list-of-key-codes

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Keycode 
    {
        KP_Enter = 0xff0d,// Enter
        XK_Escape = 0xff1b,//Escape
        XK_F11 = 0xffc8,// F11
        XK_Control_L = 0xffe3,// Left Control
        XK_u = 0x0075,// u
        XK_a = 0x0061,// a
        XK_c = 0x0063,// c
        XK_f = 0x0066,// f
        XK_w = 0x0077,// w
        XK_Left = 0x08fb,// Left Arrow
        XK_Right = 0x08fd,// Right Arrow
        XK_Shift_L = 0xffe1,// Left Shift
        XK_End = 0xff57,//End
    }
}
