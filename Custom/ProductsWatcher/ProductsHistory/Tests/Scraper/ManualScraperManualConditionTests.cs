using System;
using System.Collections.Generic;
using Radiant.Common.Business;
using Radiant.Common.Serialization;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.Custom.ProductsHistory.Scraper;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.Scrapers;
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
               },
               new ManualScraperSequenceItemByClipboard()
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
                ValueParser = new ProductDOMParserItem
                {
                    RegexPattern = "(.*)",
                    ParserItemTarget = ProductParserItemTarget.Title,
                    RegexMatch = RegexItemResultMatch.Last,
                    Target = RegexItemResultTarget.Group0Value
                }
            };

            // We'll take a screenshot while we're at it for possible manual reference
            ManualScraper _ManualScraper = new ManualScraper();
            ProductTargetScraper _ProductScraper = new ProductTargetScraper();

            // Assert that it doesn't crash
            _ManualScraper.GetTargetValueFromUrl(Browser.Firefox, "www.perdu.com", _ProductScraper, new List<IScraperItemParser> { _ItemParserWithCondition }, null);

            _ItemParserWithCondition.ValueParser.ValueCondition = new StringValueCondition
            {
                Value = "panique",
                InvariantCase = true,
                Match = StringValueCondition.MatchCondition.DontMatch
            };

            _ManualScraper.GetTargetValueFromUrl(Browser.Firefox, "www.perdu.com", _ProductScraper, new List<IScraperItemParser> { _ItemParserWithCondition }, null);
        }
    }
}
