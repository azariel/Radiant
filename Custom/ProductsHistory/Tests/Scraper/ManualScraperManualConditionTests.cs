﻿using System;
using System.Collections.Generic;
using Radiant.Common.Business;
using Radiant.Common.Serialization;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.Custom.ProductsHistory.Scraper;
using Radiant.WebScraper;
using Radiant.WebScraper.Scrapers.Conditions;
using Radiant.WebScraper.Scrapers.Manual;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.Scraper
{
    public class ManualScraperManualConditionTests
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private List<ManualScraperSequenceItem> GetCtrAAndCtrCManualSequenceCollection() =>
           new()
           {
               new ManualScraperSequenceItemByInput
               {
                   InputType = InputsManager.InputType.Keyboard,
                   InputParam = new KeyboardKeyStrokeActionInputParam
                   {
                       Delay = 50,
                       KeyStrokeCodes = new[]
                       {
                           Keycode.XK_Control_L,
                           Keycode.XK_a
                       }
                   }
               },
               new ManualScraperSequenceItemByInput
               {
                   InputType = InputsManager.InputType.Keyboard,
                   InputParam = new KeyboardKeyStrokeActionInputParam
                   {
                       Delay = 50,
                       KeyStrokeCodes = new[]
                       {
                           Keycode.XK_Control_L,
                           Keycode.XK_c
                       }
                   }
               }, new ManualScraperSequenceItemByClipboard()
               {
                   Operation = ManualScraperSequenceItemByClipboard.ClipboardOperation.Get
               }
           };

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void BasicManualConditionTest()
        {
            ManualScraperProductParser _ItemParserWithCondition = new ManualScraperProductParser
            {
                IfUrlContains = "perdu.com",
                ManualScraperSequenceItems = GetCtrAAndCtrCManualSequenceCollection(),
                Condition = new ManualScraperManualCondition
                {
                    ManualScraperSequenceItems = GetCtrAAndCtrCManualSequenceCollection(),
                    ExpectedValue = "va vous",
                    RegexPatternToApplyOnValue = "Pas de panique.*?on (.*?) aide",
                    Target = RegexItemResultTarget.Group1Value,
                    ValueStringComparison = StringComparison.InvariantCultureIgnoreCase
                },
                Target = ProductParserItemTarget.Price,
                ValueParser = null
            };

            // We'll take a screenshot while we're at it for possible manual reference
            ManualScraper _ManualScraper = new ManualScraper();
            ProductTargetScraper _ProductScraper = new ProductTargetScraper();

            // Assert that it doesn't crash
            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, "www.perdu.com", _ProductScraper, new List<ManualScraperItemParser> { _ItemParserWithCondition }, null);
        }
    }
}
