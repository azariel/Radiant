using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Radiant.InputsManager
{
    // Tightly tied to keysyms https://wiki.linuxquestions.org/wiki/List_of_keysyms or https://www.oreilly.com/library/view/xlib-reference-manual/9780937175262/16_appendix-h.html or https://gitlab.com/cunidev/gestures/-/wikis/xdotool-list-of-key-codes

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Keycode 
    {
        KP_Enter = 0xff0d,// Enter
        XK_Escape = 0xff1b,//Escape
        XK_F1 = 0xffbe,// F1
        XK_F2 = 0xffbf,// F2
        XK_F3 = 0xffc0,// F3
        XK_F4 = 0xffc1,// F4
        XK_F5 = 0xffc2,// F5
        XK_F6 = 0xffc3,// F6
        XK_F7 = 0xffc4,// F7
        XK_F8 = 0xffc5,// F8
        XK_F9 = 0xffc6,// F9
        XK_F10 = 0xffc7,// F10
        XK_F11 = 0xffc8,// F11
        XK_F12 = 0xffc9,// F12
        XK_Alt_L = 0xffe9,// L-ALT
        XK_TAB = 0xff09,// TAB
        XK_Control_L = 0xffe3,// Left Control
        XK_a = 0x0061,// a
        XK_b = 0x0062,// b
        XK_c = 0x0063,// c
        XK_d = 0x0064,// d
        XK_e = 0x0065,// e
        XK_f = 0x0066,// f
        XK_g = 0x0067,// g
        XK_h = 0x0068,// h
        XK_i = 0x0069,// i
        XK_j = 0x006a,// j
        XK_k = 0x006b,// k
        XK_l = 0x006c,// l
        XK_m = 0x006d,// m
        XK_n = 0x006e,// n
        XK_o = 0x006f,// o
        XK_p = 0x0070,// p
        XK_q = 0x0071,// q
        XK_r = 0x0072,// r
        XK_s = 0x0073,// s
        XK_t = 0x0074,// t
        XK_u = 0x0075,// u
        XK_v = 0x0076,// v
        XK_w = 0x0077,// w
        XK_x = 0x0078,// x
        XK_y = 0x0079,// y
        XK_z = 0x007a,// z
        XK_Left = 0x08fb,// Left Arrow
        XK_Right = 0x08fd,// Right Arrow
        XK_Down = 0x08fe,// Down Arrow
        XK_Up = 0x08fc,// Up Arrow
        XK_Shift_L = 0xffe1,// Left Shift
        XK_End = 0xff57,//End
        XK_Home = 0xff50,//Home
        XK_KP_Space = 0xff80,//Space
    }
}
