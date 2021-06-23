using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RadiantInputsManager
{
    // Tightly tied to keysyms https://wiki.linuxquestions.org/wiki/List_of_keysyms or https://www.oreilly.com/library/view/xlib-reference-manual/9780937175262/16_appendix-h.html

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Keycode 
    {
        KP_Enter,// Enter
        XK_Escape,//Escape
        XK_F11,// F11
        CtrlL,// Left Control
        XK_u,// u
        XK_a,// a
        XK_c,// c
        XK_f,// f
        XK_w,// w
        XK_Right,// Right Arrow
        XK_Shift_L,// Left Shift
        XK_End,//End
    }
}
