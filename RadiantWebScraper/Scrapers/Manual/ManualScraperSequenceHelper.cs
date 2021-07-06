using System.Threading;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;

namespace Radiant.WebScraper.Scrapers.Manual
{
    public static class ManualScraperSequenceHelper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// CTR+w
        /// </summary>
        public static void CloseCurrentTab()
        {
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 290,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_w
                }
            });
        }

        /// <summary>
        /// CTR+A, CTR+C
        /// </summary>
        /// <param name="aDelayBetweenSelectAllAndCopyToClipboardInMs"></param>
        public static void CopyAllToClipboard(int aDelayBetweenSelectAllAndCopyToClipboardInMs = 500)
        {
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 160,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_a
                }
            });

            Thread.Sleep(aDelayBetweenSelectAllAndCopyToClipboardInMs);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 260,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_c
                }
            });

            Thread.Sleep(100);
        }

        /// <summary>
        /// CTR+F, type text, ENTER
        /// </summary>
        /// <param name="aTextToSearch"></param>
        public static void Search(string aTextToSearch)
        {
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 89,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_f
                }
            });

            Thread.Sleep(450);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardTypeActionInputParam
            {
                Delay = 115,
                ValueToType = aTextToSearch
            });

            Thread.Sleep(325);
        }
    }
}
