﻿using System.Threading;
using Radiant.InputsManager;
using Radiant.InputsManager.InputsParam;

namespace Radiant.WebScraper.RadiantWebScraper.Scrapers.Manual
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
            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
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
            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 160,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_a
                }
            });

            Thread.Sleep(aDelayBetweenSelectAllAndCopyToClipboardInMs);

            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
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
            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 89,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_f
                }
            });

            Thread.Sleep(450);

            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardTypeActionInputParam
            {
                Delay = 115,
                ValueToType = aTextToSearch
            });

            Thread.Sleep(325);
        }
    }
}
